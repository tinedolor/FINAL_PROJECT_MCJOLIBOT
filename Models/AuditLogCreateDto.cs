using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Api.Models
{
    public class AuditLogCreateDto
    {
        [Required]
        public int userId { get; set; }

        [Required]
        [MaxLength(50)]
        public string username { get; set; }

        [Required]
        [MaxLength(20)]
        public string userRole { get; set; }

        [Required]
        [MaxLength(500)]
        public string action { get; set; }

        [Required]
        public string timestamp { get; set; }

        [MaxLength(50)]
        public string ipAddress { get; set; }
    }
} 