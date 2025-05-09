public class Remark
{
    public int Id { get; set; }
    public int TicketId { get; set; }
    public int UserId { get; set; }
    public string Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}