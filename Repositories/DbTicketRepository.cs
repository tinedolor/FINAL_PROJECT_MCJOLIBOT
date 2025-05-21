using Helpdesk.Api.Data;
using Helpdesk.Api.Interfaces;
using Helpdesk.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Api.Repositories
{
    public class DbTicketRepository : ITicketRepository
    {
        private readonly HelpdeskDbContext _context;

        public DbTicketRepository(HelpdeskDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Ticket> GetAll()
        {
            return _context.Tickets
                .Include(t => t.Remarks)
                    .ThenInclude(r => r.User)
                .Include(t => t.Department)
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .AsNoTracking()
                .ToList();
        }

        public IEnumerable<Ticket> GetByDepartment(int departmentId)
        {
            return _context.Tickets
                .Include(t => t.Remarks)
                    .ThenInclude(r => r.User)
                .Include(t => t.Department)
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .Where(t => t.DepartmentId == departmentId)
                .AsNoTracking()
                .ToList();
        }

        public Ticket GetById(int id)
        {
            return _context.Tickets
                .Include(t => t.Remarks)
                    .ThenInclude(r => r.User)
                .Include(t => t.Department)
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .AsNoTracking()
                .FirstOrDefault(t => t.Id == id);
        }

        public void Add(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            _context.SaveChanges();
        }

        public void Update(Ticket ticket)
        {
            _context.Entry(ticket).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public void AddRemark(Remark remark)
        {
            _context.Remarks.Add(remark);
            _context.SaveChanges();
        }
    }
}