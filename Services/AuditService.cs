using System.Text.Json;
using Helpdesk.Api.Data;
using Helpdesk.Api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Helpdesk.Api.Services
{
    public class AuditService
    {
        private readonly HelpdeskDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditService> _logger;

        public AuditService(
            HelpdeskDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task LogLoginAsync(int userId, bool success)
        {
            try
            {
                var log = new AuditLog
                {
                    UserId = userId,
                    EventType = "Login",
                    Timestamp = DateTime.UtcNow,
                    Details = $"Login attempt {(success ? "successful" : "failed")}",
                    IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                };

                _context.AuditLogs.Add(log);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging login for user {UserId}", userId);
            }
        }

        public async Task LogSignupAsync(int userId)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    EventType = "Signup",
                    Timestamp = DateTime.UtcNow,
                    UserId = userId,
                    Details = "User signed up",
                    IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging signup for user {UserId}", userId);
            }
        }

        public async Task LogTicketCreatedAsync(int ticketId, int userId)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    EventType = "TicketCreated",
                    Timestamp = DateTime.UtcNow,
                    UserId = userId,
                    EntityType = "Ticket",
                    EntityId = ticketId,
                    Details = "Ticket created",
                    IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging ticket creation for ticket {TicketId}", ticketId);
            }
        }

        public async Task LogTicketUpdatedAsync(int ticketId, int userId, object oldValues, object newValues)
        {
            try
            {
                var auditLog = new AuditLog
                {
                    EventType = "TicketUpdated",
                    Timestamp = DateTime.UtcNow,
                    UserId = userId,
                    EntityType = "Ticket",
                    EntityId = ticketId,
                    Details = "Ticket updated",
                    OldValues = JsonSerializer.Serialize(oldValues),
                    NewValues = JsonSerializer.Serialize(newValues),
                    IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging ticket update for ticket {TicketId}", ticketId);
            }
        }

        public async Task LogProfileUpdateAsync(int userId)
        {
            var log = new AuditLog
            {
                UserId = userId,
                EventType = "ProfileUpdate",
                Timestamp = DateTime.UtcNow,
                Details = "User profile updated",
                IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task LogPasswordChangeAsync(int userId)
        {
            var log = new AuditLog
            {
                UserId = userId,
                EventType = "PasswordChange",
                Timestamp = DateTime.UtcNow,
                Details = "User password changed",
                IpAddress = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString()
            };

            _context.AuditLogs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
} 