using Helpdesk.Api.Models;

namespace Helpdesk.Api.Interfaces;

public interface IRemarkRepository
{
    IEnumerable<Remark> GetByTicket(int ticketId);
    void Add(Remark remark);
}