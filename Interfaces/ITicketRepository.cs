using Helpdesk.Api.Models;
using System.Collections.Generic;

namespace Helpdesk.Api.Interfaces
{
    public interface ITicketRepository
    {
        IEnumerable<Ticket> GetAll();
        IEnumerable<Ticket> GetByDepartment(int departmentId);
        Ticket GetById(int id);
        void Add(Ticket ticket);
        void Update(Ticket ticket);
        void AddRemark(Remark remark);
    }
}