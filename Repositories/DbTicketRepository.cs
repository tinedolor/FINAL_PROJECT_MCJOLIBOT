using Microsoft.EntityFrameworkCore;


public class DbTicketRepository : ITicketRepository
{
    private readonly HelpdeskDbContext _context;

    public DbTicketRepository(HelpdeskDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Ticket> GetAll() => _context.Tickets.ToList();

    public Ticket GetById(int id) => _context.Tickets.FirstOrDefault(t => t.Id == id);

    public void Add(Ticket ticket)
    {
        ticket.CreatedAt = DateTime.UtcNow;
        _context.Tickets.Add(ticket);
        _context.SaveChanges();
    }

    public void Update(Ticket ticket)
    {
        var existing = GetById(ticket.Id);
        if (existing == null) return;

        existing.Title = ticket.Title;
        existing.Description = ticket.Description;
        existing.Severity = ticket.Severity;
        existing.Status = ticket.Status;
        existing.AssignedTo = ticket.AssignedTo;
        existing.DepartmentId = ticket.DepartmentId;
        existing.UpdatedAt = DateTime.UtcNow;

        _context.SaveChanges();
    }

    public IEnumerable<Ticket> GetByDepartment(int departmentId) =>
        _context.Tickets.Where(t => t.DepartmentId == departmentId).ToList();

    public IEnumerable<Ticket> GetAssignedTickets(int userId) =>
        _context.Tickets.Where(t => t.AssignedTo == userId).ToList();
}