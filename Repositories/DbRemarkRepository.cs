using Helpdesk.Api.Data;
using Helpdesk.Api.Interfaces;
using Helpdesk.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Api.Repositories;

public class DbRemarkRepository : IRemarkRepository
{
    private readonly HelpdeskDbContext _context;

    public DbRemarkRepository(HelpdeskDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Remark> GetByTicket(int ticketId)
    {
        return _context.Remarks
            .Where(r => r.TicketId == ticketId)
            .OrderByDescending(r => r.CreatedAt)
            .ToList();
    }

    public void Add(Remark remark)
    {
        _context.Remarks.Add(remark);
        _context.SaveChanges();
    }
} 