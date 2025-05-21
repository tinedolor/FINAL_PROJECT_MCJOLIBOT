using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Helpdesk.Api.Models;

namespace Helpdesk.Api.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string EventType { get; set; } = string.Empty;  // Login, Signup, TicketCreated, TicketUpdated, etc.

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public int UserId { get; set; }  // The user who performed the action

        [MaxLength(50)]
        public string? EntityType { get; set; }  // Type of entity affected (e.g., "Ticket", "User")
        
        public int? EntityId { get; set; }  // ID of the affected entity

        [Required]
        [MaxLength(500)]
        public string Details { get; set; } = string.Empty;  // Additional details about the event

        [MaxLength(1000)]
        public string? OldValues { get; set; }  // JSON string of old values (for updates)
        
        [MaxLength(1000)]
        public string? NewValues { get; set; }  // JSON string of new values (for updates)

        [MaxLength(50)]
        public string? IpAddress { get; set; }  // IP address of the user

        // Navigation property
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
} 