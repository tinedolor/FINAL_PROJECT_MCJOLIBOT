using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Helpdesk.Api.Models
{
    public class Remark
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TicketId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Comment { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [JsonIgnore]
        [ForeignKey("TicketId")]
        public virtual Ticket Ticket { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public Remark()
        {
            CreatedAt = DateTime.UtcNow;
        }
    }
}