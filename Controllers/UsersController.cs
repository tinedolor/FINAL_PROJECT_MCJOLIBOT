using Helpdesk.Api.Interfaces;
using Helpdesk.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Helpdesk.Api.Data;
using Helpdesk.Api.Services;

namespace Helpdesk.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UsersController> _logger;
    private readonly AuditService _auditService;

    public UsersController(
        IUserRepository userRepository, 
        ILogger<UsersController> logger,
        AuditService auditService)
    {
        _userRepository = userRepository;
        _logger = logger;
        _auditService = auditService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Create a new user")]
    [SwaggerResponse(201, "User created successfully")]
    [SwaggerResponse(400, "Invalid input or username already exists")]
    [SwaggerResponse(403, "Not authorized to create users")]
    public async Task<IActionResult> Create([FromBody] UserCreateDto userDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Validate role
            var validRoles = new[] { "Admin", "Supervisor", "Officer", "JuniorOfficer" };
            if (!validRoles.Contains(userDto.Role))
            {
                return BadRequest($"Invalid role. Valid roles are: {string.Join(", ", validRoles)}");
            }

            var user = _userRepository.Create(userDto);
            await _auditService.LogSignupAsync(user.Id);
            
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create user");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, "An error occurred while creating the user");
        }
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get user by ID")]
    [SwaggerResponse(200, "User found")]
    [SwaggerResponse(404, "User not found")]
    public IActionResult GetById(int id)
    {
        var user = _userRepository.GetById(id);
        if (user == null)
        {
            return NotFound();
        }

        // Don't return the password hash
        user.PasswordHash = null;
        return Ok(user);
    }

    [HttpGet("department/{departmentId}")]
    [SwaggerOperation(Summary = "Get users by department")]
    [SwaggerResponse(200, "Users found")]
    public IActionResult GetByDepartment(int departmentId)
    {
        var users = _userRepository.GetByDepartment(departmentId)
            .Select(u => new { u.Id, u.Username, u.FullName, u.Email, u.Role, u.DepartmentId });
        return Ok(users);
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all users")]
    [SwaggerResponse(200, "Users found")]
    public IActionResult GetAll()
    {
        var users = _userRepository.GetAll()
            .Select(u => new { u.Id, u.Username, u.FullName, u.Email, u.Role, u.DepartmentId, u.EmployeeId });
        return Ok(users);
    }

    [HttpGet("test-connection")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Test database connection and repository type")]
    public IActionResult TestConnection()
    {
        try
        {
            var repositoryType = _userRepository.GetType().Name;
            var isInMemory = repositoryType == "InMemoryUserRepository";
            var users = _userRepository.GetAll().ToList();

            return Ok(new
            {
                RepositoryType = repositoryType,
                IsInMemory = isInMemory,
                UserCount = users.Count,
                SampleUsers = users.Take(3).Select(u => new { u.Username, u.Role, u.DepartmentId })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Error = "Database connection test failed",
                Message = ex.Message,
                RepositoryType = _userRepository.GetType().Name
            });
        }
    }

    [HttpPost("create-account")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Create a new account using Employee ID")]
    [SwaggerResponse(201, "Account created successfully")]
    [SwaggerResponse(400, "Invalid input or Employee ID already registered")]
    public async Task<IActionResult> CreateAccount([FromBody] AccountCreateDto accountDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _userRepository.CreateAccount(accountDto);
            await _auditService.LogSignupAsync(user.Id);
            
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Failed to create account");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating account");
            return StatusCode(500, "An error occurred while creating the account");
        }
    }

    [HttpGet("employees")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Get all employees")]
    [SwaggerResponse(200, "Employees found")]
    public IActionResult GetAllEmployees([FromServices] HelpdeskDbContext context)
    {
        var employees = context.Employees.ToList();
        return Ok(employees);
    }
} 