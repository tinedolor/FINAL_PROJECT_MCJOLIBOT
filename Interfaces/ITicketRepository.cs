public interface ITicketRepository
{
    IEnumerable<Ticket> GetAll();
    Ticket GetById(int id);
    void Add(Ticket ticket);
    void Update(Ticket ticket);
    IEnumerable<Ticket> GetByDepartment(int departmentId);
    IEnumerable<Ticket> GetAssignedTickets(int userId);
}