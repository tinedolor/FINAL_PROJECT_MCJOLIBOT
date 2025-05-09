public class User
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } // "Admin", "Supervisor", "Officer", "JuniorOfficer"
    public int DepartmentId { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
}