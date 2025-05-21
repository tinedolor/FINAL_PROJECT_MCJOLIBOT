using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Api.Models
{
    public class TicketUpdateDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [RegularExpression("^(Low|Medium|High|Critical)$", ErrorMessage = "Severity must be Low, Medium, High, or Critical")]
        public string Severity { get; set; }

        [Required]
        [RegularExpression("^(New|Open|In Progress|Resolved|Closed)$", ErrorMessage = "Status must be New, Open, In Progress, Resolved, or Closed")]
        public string Status { get; set; }

        public int? AssignedTo { get; set; }

        public bool IsValid()
        {
            return !string.IsNullOrEmpty(Status) && TicketStatus.IsValid(Status);
        }
    }
}