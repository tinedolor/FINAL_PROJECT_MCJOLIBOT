using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Helpdesk.Api.Models;
using Helpdesk.Api.Data;
using System.Security.Claims;
using Swashbuckle.AspNetCore.Annotations;

namespace Helpdesk.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AuditLogsController : ControllerBase
    {
        private readonly HelpdeskDbContext _context;
        private readonly ILogger<AuditLogsController> _logger;

        public AuditLogsController(HelpdeskDbContext context, ILogger<AuditLogsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new audit log")]
        [SwaggerResponse(201, "Audit log created successfully")]
        [SwaggerResponse(400, "Invalid input")]
        public async Task<IActionResult> Create([FromBody] AuditLogCreateDto auditLogDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var auditLog = new AuditLog
                {
                    EventType = auditLogDto.action,
                    Timestamp = DateTime.UtcNow,
                    UserId = auditLogDto.userId,
                    Details = auditLogDto.action,
                    IpAddress = auditLogDto.ipAddress
                };

                _context.AuditLogs.Add(auditLog);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = auditLog.Id }, auditLog);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating audit log");
                return StatusCode(500, "An error occurred while creating the audit log");
            }
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all audit logs with optional filtering")]
        public async Task<ActionResult<IEnumerable<object>>> GetAll(
            [FromQuery] string eventType = null,
            [FromQuery] int? userId = null,
            [FromQuery] string entityType = null,
            [FromQuery] int? entityId = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation("Getting audit logs with filters: EventType={EventType}, UserId={UserId}, EntityType={EntityType}, EntityId={EntityId}, StartDate={StartDate}, EndDate={EndDate}",
                    eventType, userId, entityType, entityId, startDate, endDate);

                var parameters = new List<object>();
                var whereClauses = new List<string>();
                var paramIndex = 0;

                if (!string.IsNullOrEmpty(eventType))
                {
                    whereClauses.Add($"al.EventType = @p{paramIndex}");
                    parameters.Add(eventType);
                    paramIndex++;
                }

                if (userId.HasValue)
                {
                    whereClauses.Add($"al.UserId = @p{paramIndex}");
                    parameters.Add(userId.Value);
                    paramIndex++;
                }

                if (!string.IsNullOrEmpty(entityType))
                {
                    whereClauses.Add($"al.EntityType = @p{paramIndex}");
                    parameters.Add(entityType);
                    paramIndex++;
                }

                if (entityId.HasValue)
                {
                    whereClauses.Add($"al.EntityId = @p{paramIndex}");
                    parameters.Add(entityId.Value);
                    paramIndex++;
                }

                if (startDate.HasValue)
                {
                    whereClauses.Add($"al.Timestamp >= @p{paramIndex}");
                    parameters.Add(startDate.Value);
                    paramIndex++;
                }

                if (endDate.HasValue)
                {
                    whereClauses.Add($"al.Timestamp <= @p{paramIndex}");
                    parameters.Add(endDate.Value);
                    paramIndex++;
                }

                var whereClause = whereClauses.Any() 
                    ? "WHERE " + string.Join(" AND ", whereClauses)
                    : string.Empty;

                var sql = $@"
                    SELECT 
                        al.Id,
                        ISNULL(al.EventType, '') as EventType,
                        al.Timestamp,
                        al.UserId,
                        ISNULL(u.Username, '') as Username,
                        ISNULL(u.Role, '') as UserRole,
                        ISNULL(al.Details, '') as Details,
                        ISNULL(al.EntityType, '') as EntityType,
                        al.EntityId,
                        ISNULL(al.OldValues, '') as OldValues,
                        ISNULL(al.NewValues, '') as NewValues,
                        ISNULL(al.IpAddress, '') as IpAddress
                    FROM AuditLogs al
                    LEFT JOIN Users u ON al.UserId = u.Id
                    {whereClause}
                    ORDER BY al.Timestamp DESC";

                var logs = await _context.AuditLogs
                    .FromSqlRaw(sql, parameters.ToArray())
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogInformation("Retrieved {Count} audit logs", logs.Count);
                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit logs: {Message}", ex.Message);
                return StatusCode(500, $"An error occurred while retrieving audit logs: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get audit log by ID")]
        public async Task<ActionResult<object>> GetById(int id)
        {
            try
            {
                var log = await (from l in _context.AuditLogs
                               join u in _context.Users on l.UserId equals u.Id into userJoin
                               from u in userJoin.DefaultIfEmpty()
                               where l.Id == id
                               select new
                               {
                                   l.Id,
                                   l.EventType,
                                   l.Timestamp,
                                   l.UserId,
                                   Username = u.Username,
                                   UserRole = u.Role,
                                   l.EntityType,
                                   l.EntityId,
                                   l.Details,
                                   l.OldValues,
                                   l.NewValues,
                                   l.IpAddress
                               })
                               .AsNoTracking()
                               .FirstOrDefaultAsync();

                if (log == null)
                {
                    return NotFound();
                }

                return Ok(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit log {LogId}", id);
                return StatusCode(500, "An error occurred while retrieving the audit log");
            }
        }

        [HttpGet("filter")]
        [Authorize(Roles = "Admin")]
        [SwaggerOperation(Summary = "Filter audit logs by various criteria")]
        public async Task<IActionResult> Filter(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string eventType,
            [FromQuery] string username,
            [FromQuery] string entityType)
        {
            try
            {
                var query = from log in _context.AuditLogs
                           join user in _context.Users on log.UserId equals user.Id into userJoin
                           from user in userJoin.DefaultIfEmpty()
                           select new
                           {
                               log.Id,
                               log.EventType,
                               log.Timestamp,
                               Username = user.Username,
                               UserRole = user.Role,
                               log.Details,
                               log.EntityType,
                               log.EntityId,
                               log.OldValues,
                               log.NewValues,
                               log.IpAddress
                           };

                if (startDate.HasValue)
                    query = query.Where(l => l.Timestamp >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(l => l.Timestamp <= endDate.Value);

                if (!string.IsNullOrWhiteSpace(eventType))
                    query = query.Where(l => l.EventType == eventType);

                if (!string.IsNullOrWhiteSpace(username))
                    query = query.Where(l => l.Username != null && l.Username.Contains(username));

                if (!string.IsNullOrWhiteSpace(entityType))
                    query = query.Where(l => l.EntityType == entityType);

                var logs = await query
                    .OrderByDescending(l => l.Timestamp)
                    .ToListAsync();

                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering audit logs");
                return StatusCode(500, "An error occurred while filtering audit logs");
            }
        }

        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetUserAuditLogs(int userId)
        {
            try
            {
                var logs = await (from log in _context.AuditLogs
                                join user in _context.Users on log.UserId equals user.Id into userJoin
                                from user in userJoin.DefaultIfEmpty()
                                where log.UserId == userId
                                select new
                                {
                                    log.Id,
                                    log.EventType,
                                    log.Timestamp,
                                    Username = user.Username,
                                    UserRole = user.Role,
                                    log.Details,
                                    log.EntityType,
                                    log.EntityId,
                                    log.OldValues,
                                    log.NewValues,
                                    log.IpAddress
                                })
                                .OrderByDescending(log => log.Timestamp)
                                .ToListAsync();

                if (!logs.Any())
                {
                    return NotFound($"No audit logs found for user with ID {userId}");
                }

                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting audit logs for user {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving user audit logs");
            }
        }

        [HttpGet("recent")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<object>>> GetRecentAuditLogs([FromQuery] int count = 50)
        {
            try
            {
                var logs = await _context.AuditLogs
                    .FromSqlRaw(@"
                        SELECT TOP (@p0) 
                            al.Id,
                            al.UserId,
                            ISNULL(al.EventType, '') as EventType,
                            al.Timestamp,
                            ISNULL(u.Username, '') as Username,
                            ISNULL(u.Role, '') as UserRole,
                            ISNULL(al.Details, '') as Details,
                            ISNULL(al.EntityType, '') as EntityType,
                            al.EntityId,
                            ISNULL(al.OldValues, '') as OldValues,
                            ISNULL(al.NewValues, '') as NewValues,
                            ISNULL(al.IpAddress, '') as IpAddress
                        FROM AuditLogs al
                        LEFT JOIN Users u ON al.UserId = u.Id
                        ORDER BY al.Timestamp DESC", count)
                    .AsNoTracking()
                    .ToListAsync();

                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent audit logs");
                return StatusCode(500, "An error occurred while retrieving recent audit logs");
            }
        }
    }
} 