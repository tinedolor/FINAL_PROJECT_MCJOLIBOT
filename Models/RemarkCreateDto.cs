using System.ComponentModel.DataAnnotations;

namespace Helpdesk.Api.Models
{
    public class RemarkCreateDto
    {
        [Required]
        public string Comment { get; set; }
    }
}