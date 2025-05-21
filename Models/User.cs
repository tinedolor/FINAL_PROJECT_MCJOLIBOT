using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Helpdesk.Api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public int DepartmentId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string EmployeeId { get; set; }

        // Navigation properties
        public virtual Department Department { get; set; }
        
        [InverseProperty("CreatedByUser")]
        public virtual ICollection<Ticket> CreatedTickets { get; set; }
        
        [InverseProperty("AssignedToUser")]
        public virtual ICollection<Ticket> AssignedTickets { get; set; }
        
        public virtual ICollection<Remark> Remarks { get; set; }

        public User()
        {
            CreatedTickets = new List<Ticket>();
            AssignedTickets = new List<Ticket>();
            Remarks = new List<Remark>();
        }
    }
}