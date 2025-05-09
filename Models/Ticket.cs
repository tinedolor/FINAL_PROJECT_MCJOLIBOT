using System;

public class Ticket
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Severity { get; set; } // "Low", "Medium", "High", "Critical"
    public string Status { get; set; } // "Open", "InProgress", "Resolved", "Closed"
    public int CreatedBy { get; set; }
    public int? AssignedTo { get; set; }
    public int DepartmentId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
