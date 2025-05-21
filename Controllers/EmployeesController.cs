using Helpdesk.Api.Data;
using Helpdesk.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Helpdesk.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly HelpdeskDbContext _context;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(HelpdeskDbContext context, ILogger<EmployeesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all employees")]
    [SwaggerResponse(200, "Employees found")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var employees = await _context.Employees
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FirstName,
                    e.LastName,
                    e.Email,
                    e.Phone,
                    e.HireDate,
                    e.JobTitle,
                    e.Department,
                    e.Salary
                })
                .ToListAsync();

            return Ok(employees);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all employees");
            return StatusCode(500, "An error occurred while fetching employees");
        }
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get employee by ID")]
    [SwaggerResponse(200, "Employee found")]
    [SwaggerResponse(404, "Employee not found")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting employee by ID");
            return StatusCode(500, "An error occurred while fetching the employee");
        }
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new employee")]
    [SwaggerResponse(201, "Employee created successfully")]
    [SwaggerResponse(400, "Invalid input")]
    public async Task<IActionResult> Create([FromBody] Employee employee)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Ensure EmployeeId is not set
            employee.EmployeeId = 0;

            // Validate email uniqueness
            if (await _context.Employees.AnyAsync(e => e.Email == employee.Email))
            {
                ModelState.AddModelError("Email", "An employee with this email already exists");
                return BadRequest(ModelState);
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = employee.EmployeeId }, employee);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating employee");
            return StatusCode(500, "An error occurred while creating the employee");
        }
    }
} 