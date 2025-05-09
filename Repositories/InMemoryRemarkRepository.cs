public class InMemoryRemarkRepository : IRemarkRepository
{
    private readonly List<Remark> _remarks = new();
    private int _nextId = 1;

    public IEnumerable<Remark> GetByTicket(int ticketId) =>
        _remarks.Where(r => r.TicketId == ticketId).OrderBy(r => r.CreatedAt);

    public void Add(Remark remark)
    {
        remark.Id = _nextId++;
        remark.CreatedAt = DateTime.UtcNow;
        _remarks.Add(remark);
    }
}
4.