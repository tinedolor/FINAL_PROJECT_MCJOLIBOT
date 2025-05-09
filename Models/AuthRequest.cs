using System.ComponentModel.DataAnnotations.Schema;

namespace Helpdesk.Api.Models
{
public class AuthRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
}
}