using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Api.Models;

public class AccountCreateDto
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
    public string EmployeeId { get; set; }
} 