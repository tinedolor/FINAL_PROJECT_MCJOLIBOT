public interface IRemarkRepository
{
    IEnumerable<Remark> GetByTicket(int ticketId);
    void Add(Remark remark);
}
