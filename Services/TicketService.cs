using Helpdesk.Api.Interfaces;
using Helpdesk.Api.Models;
using Microsoft.AspNetCore.Authorization;

namespace Helpdesk.Api.Services;

public class TicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserRepository _userRepository;
    private readonly AuditService _auditService;

    public TicketService(
        ITicketRepository ticketRepository,
        IUserRepository userRepository,
        AuditService auditService)
    {
        _ticketRepository = ticketRepository;
        _userRepository = userRepository;
        _auditService = auditService;
    }

    public bool CanAssignTicket(int userId, Ticket ticket)
    {
        var user = _userRepository.GetById(userId);
        if (user == null) return false;

        // Admin can assign any ticket
        if (user.Role == "Admin") return true;

        // Supervisors can assign any ticket in their department
        if (user.Role == "Supervisor" && user.DepartmentId == ticket.DepartmentId) return true;

        // Junior Officers cannot work on Critical tickets unless assigned by Supervisor
        if (user.Role == "JuniorOfficer" && ticket.Severity == "Critical")
        {
            return false;
        }

        // Officers can work on any ticket in their department
        if (user.Role == "Officer" && user.DepartmentId == ticket.DepartmentId)
        {
            return true;
        }

        return false;
    }

    public bool CanReassignTicket(int userId, Ticket ticket, int newDepartmentId)
    {
        var user = _userRepository.GetById(userId);
        if (user == null) return false;

        // Only Admin and Supervisors can reassign tickets between departments
        if (user.Role != "Admin" && user.Role != "Supervisor") return false;

        // Supervisors can only reassign tickets within their department
        if (user.Role == "Supervisor" && user.DepartmentId != ticket.DepartmentId) return false;

        return true;
    }

    public bool CanUpdateTicket(int userId, Ticket ticket)
    {
        var user = _userRepository.GetById(userId);
        if (user == null) return false;

        // Admin can update any ticket
        if (user.Role == "Admin") return true;

        // Users can only update tickets in their department
        if (user.DepartmentId != ticket.DepartmentId) return false;

        // Junior Officers cannot update Critical tickets
        if (user.Role == "JuniorOfficer" && ticket.Severity == "Critical")
        {
            return false;
        }

        return true;
    }

    public async Task<bool> AssignTicketAsync(int ticketId, int assignedToId, int assignedById)
    {
        var ticket = _ticketRepository.GetById(ticketId);
        if (ticket == null) return false;

        var assignedBy = _userRepository.GetById(assignedById);
        if (assignedBy == null) return false;

        var assignedTo = _userRepository.GetById(assignedToId);
        if (assignedTo == null) return false;

        // Check if the assigned user is in the same department as the ticket
        if (assignedTo.DepartmentId != ticket.DepartmentId)
        {
            return false;
        }

        if (!CanAssignTicket(assignedById, ticket))
        {
            return false;
        }

        ticket.AssignedTo = assignedToId;
        ticket.UpdatedAt = DateTime.UtcNow;
        _ticketRepository.Update(ticket);

        await _auditService.LogTicketUpdatedAsync(
            assignedById,
            ticketId,
            "Ticket assigned",
            new { AssignedTo = assignedToId }
        );

        return true;
    }

    public async Task<bool> ReassignTicketAsync(int ticketId, int newDepartmentId, int reassignedById)
    {
        var ticket = _ticketRepository.GetById(ticketId);
        if (ticket == null) return false;

        var reassignedBy = _userRepository.GetById(reassignedById);
        if (reassignedBy == null) return false;

        if (!CanReassignTicket(reassignedById, ticket, newDepartmentId))
        {
            return false;
        }

        var oldDepartmentId = ticket.DepartmentId;
        ticket.DepartmentId = newDepartmentId;
        ticket.AssignedTo = null; // Clear assignment when reassigning to new department
        ticket.UpdatedAt = DateTime.UtcNow;
        _ticketRepository.Update(ticket);

        await _auditService.LogTicketUpdatedAsync(
            reassignedById,
            ticketId,
            "Ticket reassigned to different department",
            new { OldDepartmentId = oldDepartmentId, NewDepartmentId = newDepartmentId }
        );

        return true;
    }
} 