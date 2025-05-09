[Authorize]
[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IRemarkRepository _remarkRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<TicketsController> _logger;

    public TicketsController(
        ITicketRepository ticketRepository,
        IRemarkRepository remarkRepository,
        IUserRepository userRepository,
        ILogger<TicketsController> logger)
    {
        _ticketRepository = ticketRepository;
        _remarkRepository = remarkRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var departmentId = int.Parse(User.FindFirst("DepartmentId").Value);
        var role = User.FindFirst(ClaimTypes.Role).Value;

        if (role == "Admin")
            return Ok(_ticketRepository.GetAll());

        return Ok(_ticketRepository.GetByDepartment(departmentId));
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var ticket = _ticketRepository.GetById(id);
        if (ticket == null)
            return NotFound();

        var departmentId = int.Parse(User.FindFirst("DepartmentId").Value);
        var role = User.FindFirst(ClaimTypes.Role).Value;

        if (role != "Admin" && ticket.DepartmentId != departmentId)
            return Forbid();

        return Ok(ticket);
    }

    [HttpPost]
    public IActionResult Create([FromBody] Ticket ticket)
    {
        var userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value);
        ticket.CreatedBy = userId;
        ticket.Status = "Open";

        _ticketRepository.Add(ticket);

        // Add initial remark
        _remarkRepository.Add(new Remark
        {
            TicketId = ticket.Id,
            UserId = userId,
            Comment = "Ticket created"
        });

        return CreatedAtAction(nameof(GetById), new { id = ticket.Id }, ticket);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Ticket ticket)
    {
        if (id != ticket.Id)
            return BadRequest();

        var existing = _ticketRepository.GetById(id);
        if (existing == null)
            return NotFound();

        var userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value);
        var role = User.FindFirst(ClaimTypes.Role).Value;
        var departmentId = int.Parse(User.FindFirst("DepartmentId").Value);

        // Authorization checks
        if (role != "Admin" && existing.DepartmentId != departmentId)
            return Forbid();

        // Junior officers can't work on Critical tickets unless assigned by supervisor
        if (role == "JuniorOfficer" && ticket.Severity == "Critical" &&
            (existing.AssignedTo != userId || existing.Severity != "Critical"))
            return BadRequest("Junior officers cannot work on Critical tickets unless specifically assigned");

        _ticketRepository.Update(ticket);

        // Add remark about update
        _remarkRepository.Add(new Remark
        {
            TicketId = ticket.Id,
            UserId = userId,
            Comment = $"Ticket updated: Status={ticket.Status}, Severity={ticket.Severity}, AssignedTo={ticket.AssignedTo}"
        });

        return NoContent();
    }

    [HttpGet("{id}/remarks")]
    public IActionResult GetRemarks(int id)
    {
        var ticket = _ticketRepository.GetById(id);
        if (ticket == null)
            return NotFound();

        var departmentId = int.Parse(User.FindFirst("DepartmentId").Value);
        var role = User.FindFirst(ClaimTypes.Role).Value;

        if (role != "Admin" && ticket.DepartmentId != departmentId)
            return Forbid();

        return Ok(_remarkRepository.GetByTicket(id));
    }

    [HttpPost("{id}/remarks")]
    public IActionResult AddRemark(int id, [FromBody] Remark remark)
    {
        var ticket = _ticketRepository.GetById(id);
        if (ticket == null)
            return NotFound();

        var userId = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value);
        var departmentId = int.Parse(User.FindFirst("DepartmentId").Value);
        var role = User.FindFirst(ClaimTypes.Role).Value;

        if (role != "Admin" && ticket.DepartmentId != departmentId)
            return Forbid();

        remark.TicketId = id;
        remark.UserId = userId;
        _remarkRepository.Add(remark);

        return CreatedAtAction(nameof(GetRemarks), new { id }, remark);
    }
}