using Helpdesk.Api.Interfaces;
using Helpdesk.Api.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace Helpdesk.Api.Services;
public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    private readonly AuditService _auditService;

    public AuthService(
        IUserRepository userRepository, 
        IConfiguration configuration,
        AuditService auditService)
    {
        _userRepository = userRepository;
        _configuration = configuration;
        _auditService = auditService;
    }

    public async Task<AuthResponse> AuthenticateAsync(AuthRequest request)
    {
        var user = _userRepository.GetByUsername(request.Username);
        var success = user != null && BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (user != null)
        {
            await _auditService.LogLoginAsync(user.Id, success);
        }

        if (!success)
            return null;

        var token = GenerateJwtToken(user);
        return new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Role = user.Role,
            DepartmentId = user.DepartmentId
        };
    }

    private string GenerateJwtToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("DepartmentId", user.DepartmentId.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}