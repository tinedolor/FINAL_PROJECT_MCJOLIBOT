using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Api.Models;

public class UserCreateDto
{
    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    [Required]
    [StringLength(100)]
    public string Password { get; set; }

    [Required]
    [StringLength(100)]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }

    [Required]
    public string Role { get; set; }

    [Required]
    public int DepartmentId { get; set; }
} 