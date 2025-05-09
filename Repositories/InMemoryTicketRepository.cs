public class InMemoryTicketRepository : ITicketRepository
{
    private readonly List<Ticket> _tickets = new();
    private int _nextId = 1;

    public IEnumerable<Ticket> GetAll() => _tickets;

    public Ticket GetById(int id) => _tickets.FirstOrDefault(t => t.Id == id);

    public void Add(Ticket ticket)
    {
        ticket.Id = _nextId++;
        ticket.CreatedAt = DateTime.UtcNow;
        _tickets.Add(ticket);
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
    }

    public IEnumerable<Ticket> GetByDepartment(int departmentId) =>
        _tickets.Where(t => t.DepartmentId == departmentId);

    public IEnumerable<Ticket> GetAssignedTickets(int userId) =>
        _tickets.Where(t => t.AssignedTo == userId);
}
