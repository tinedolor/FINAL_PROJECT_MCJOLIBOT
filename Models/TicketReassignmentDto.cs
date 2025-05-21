using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Api.Models
{
    public class TicketReassignmentDto
    {
        [Required]
        public int NewDepartmentId { get; set; }
    }
} 