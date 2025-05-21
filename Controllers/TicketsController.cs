using Helpdesk.Api.Data;
using Helpdesk.Api.Interfaces;
using Helpdesk.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.Extensions.Logging;
using Helpdesk.Api.Services;

namespace Helpdesk.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly HelpdeskDbContext _context;
    private readonly ILogger<TicketsController> _logger;
    private readonly AuditService _auditService;

    public TicketsController(
        HelpdeskDbContext context,
        ILogger<TicketsController> logger,
        AuditService auditService)
    {
        _context = context;
        _logger = logger;
        _auditService = auditService;
    }

    [HttpGet]
    [SwaggerOperation(Summary = "Get all tickets")]
    public async Task<ActionResult<IEnumerable<Ticket>>> GetAll()
    {
        try
        {
            var tickets = await _context.Tickets
                .Include(t => t.Remarks)
                    .ThenInclude(r => r.User)
                .Include(t => t.Department)
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .AsNoTracking()
                .ToListAsync();

            return Ok(tickets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tickets");
            return StatusCode(500, "An error occurred while retrieving tickets");
        }
    }

    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get ticket by ID")]
    public async Task<ActionResult<Ticket>> GetById(int id)
    {
        try
        {
            var ticket = await _context.Tickets
                .Include(t => t.Remarks)
                    .ThenInclude(r => r.User)
                .Include(t => t.Department)
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
            {
                return NotFound();
            }

            return Ok(ticket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting ticket {TicketId}", id);
            return StatusCode(500, "An error occurred while retrieving the ticket");
        }
    }

    [HttpPost]
    [SwaggerOperation(Summary = "Create a new ticket")]
    public async Task<ActionResult<Ticket>> Create(TicketCreateDto ticketDto)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ticket = new Ticket
            {
                Title = ticketDto.Title,
                Description = ticketDto.Description,
                Severity = ticketDto.Severity,
                Status = ticketDto.Status,
                AssignedTo = ticketDto.AssignedTo,
                CreatedBy = int.Parse(User.FindFirst("UserId")?.Value ?? "0"),
                DepartmentId = int.Parse(User.FindFirst("DepartmentId")?.Value ?? "0"),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            await _auditService.LogTicketCreatedAsync(ticket.Id, ticket.CreatedBy);

            return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating ticket");
            return StatusCode(500, "An error occurred while creating the ticket");
        }
    }

    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update a ticket")]
    public async Task<IActionResult> Update(int id, TicketUpdateDto ticketDto)
    {
        try
        {
            if (id != ticketDto.Id)
            {
                return BadRequest("ID mismatch");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }

            var oldValues = new
            {
                ticket.Title,
                ticket.Description,
                ticket.Severity,
                ticket.Status,
                ticket.AssignedTo
            };

            ticket.Title = ticketDto.Title;
            ticket.Description = ticketDto.Description;
            ticket.Severity = ticketDto.Severity;
            ticket.Status = ticketDto.Status;
            ticket.AssignedTo = ticketDto.AssignedTo;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _auditService.LogTicketUpdatedAsync(
                ticket.Id,
                int.Parse(User.FindFirst("UserId")?.Value ?? "0"),
                oldValues,
                new
                {
                    ticket.Title,
                    ticket.Description,
                    ticket.Severity,
                    ticket.Status,
                    ticket.AssignedTo
                });

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating ticket {TicketId}", id);
            return StatusCode(500, "An error occurred while updating the ticket");
        }
    }

    [HttpPost("{id}/remarks")]
    [SwaggerOperation(Summary = "Add a remark to a ticket")]
    public async Task<ActionResult<Remark>> AddRemark(int id, RemarkCreateDto remarkDto)
    {
        try
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound("Ticket not found");
            }

            var remark = new Remark
            {
                TicketId = id,
                UserId = int.Parse(User.FindFirst("UserId")?.Value ?? "0"),
                Comment = remarkDto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Remarks.Add(remark);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id }, remark);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding remark to ticket {TicketId}", id);
            return StatusCode(500, "An error occurred while adding the remark");
        }
    }
} 