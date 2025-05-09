using System.ComponentModel.DataAnnotations.Schema;

namespace Helpdesk.Api.Models
{
public class AuthResponse
{
    public string Token { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; }
    public int DepartmentId { get; set; }
}
}