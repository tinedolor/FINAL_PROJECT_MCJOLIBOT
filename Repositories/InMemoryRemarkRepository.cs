using Helpdesk.Api.Models;
using Helpdesk.Api.Data;
using Helpdesk.Api.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Helpdesk.Api.Repositories
{
    public class InMemoryRemarkRepository : IRemarkRepository
    {
        private readonly HelpdeskDbContext _context;

        public InMemoryRemarkRepository(HelpdeskDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Remark> GetByTicket(int ticketId)
        {
            return _context.Remarks
                .Where(r => r.TicketId == ticketId)
                .Include(r => r.User) 
                .OrderBy(r => r.CreatedAt) 
                .AsNoTracking()
                .ToList();
        }

        public void Add(Remark remark)
        {
            remark.CreatedAt = DateTime.UtcNow;
            _context.Remarks.Add(remark);
            _context.SaveChanges();
        }
    }
}