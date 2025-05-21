using System.Collections.Generic;

namespace Helpdesk.Api.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Navigation properties
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }

        public Department()
        {
            Users = new List<User>();
            Tickets = new List<Ticket>();
        }
    }
}