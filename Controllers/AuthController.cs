using Helpdesk.Api.Models;
using Helpdesk.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Helpdesk.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(AuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Authenticate user and get JWT token")]
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        var response = await _authService.AuthenticateAsync(request);
        if (response == null)
            return Unauthorized(new { message = "Username or password is incorrect" });

        return Ok(response);
    }
} 