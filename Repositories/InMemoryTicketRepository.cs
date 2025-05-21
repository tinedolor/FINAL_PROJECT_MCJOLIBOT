using Helpdesk.Api.Interfaces;
using Helpdesk.Api.Models;
using System.Collections.Generic;
using System.Linq;

namespace Helpdesk.Api.Repositories
{
    public class InMemoryTicketRepository : ITicketRepository
    {
        private readonly List<Ticket> _tickets = new List<Ticket>();
        private readonly List<Remark> _remarks = new List<Remark>();
        private int _nextTicketId = 1;
        private int _nextRemarkId = 1;

        public IEnumerable<Ticket> GetAll()
        {
            return _tickets;
        }

        public IEnumerable<Ticket> GetByDepartment(int departmentId)
        {
            return _tickets.Where(t => t.DepartmentId == departmentId);
        }

        public Ticket GetById(int id)
        {
            var ticket = _tickets.FirstOrDefault(t => t.Id == id);
            if (ticket != null)
            {
                ticket.Remarks = _remarks.Where(r => r.TicketId == id).ToList();
            }
            return ticket;
        }

        public void Add(Ticket ticket)
        {
            ticket.Id = _nextTicketId++;
            _tickets.Add(ticket);
        }

        public void Update(Ticket ticket)
        {
            var index = _tickets.FindIndex(t => t.Id == ticket.Id);
            if (index != -1)
            {
                _tickets[index] = ticket;
            }
        }

        public void AddRemark(Remark remark)
        {
            remark.Id = _nextRemarkId++;
            _remarks.Add(remark);
        }
    }
}