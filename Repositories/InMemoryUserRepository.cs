using Microsoft.EntityFrameworkCore;

public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new()
    {
        new User { Id = 1, Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), Role = "Admin", DepartmentId = 1 },
        new User { Id = 2, Username = "supervisor1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("sup123"), Role = "Supervisor", DepartmentId = 1 },
        new User { Id = 3, Username = "officer1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("off123"), Role = "Officer", DepartmentId = 1 },
        new User { Id = 4, Username = "junior1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("jun123"), Role = "JuniorOfficer", DepartmentId = 1 },
    };

    public User GetByUsername(string username) =>
        _users.FirstOrDefault(u => u.Username == username);

    public User GetById(int id) =>
        _users.FirstOrDefault(u => u.Id == id);

    public IEnumerable<User> GetByDepartment(int departmentId) =>
        _users.Where(u => u.DepartmentId == departmentId);
}