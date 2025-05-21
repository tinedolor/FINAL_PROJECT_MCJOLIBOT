using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Api.Models;

public class UserUpdateDto
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    [StringLength(100)]
    public string Email { get; set; }
} 