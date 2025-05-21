using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Api.Models
{
    public class TicketAssignmentDto
    {
        [Required]
        public int AssignedToId { get; set; }
    }
} 