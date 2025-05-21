using Helpdesk.Api.Interfaces;
using Helpdesk.Api.Models;
using BCrypt.Net;

namespace Helpdesk.Api.Repositories;
public class InMemoryUserRepository : IUserRepository
{
    private readonly List<User> _users = new()
    {
        new User { Id = 1, Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), Role = "Admin", DepartmentId = 1, EmployeeId = "EMP001" },
        new User { Id = 2, Username = "supervisor1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("sup123"), Role = "Supervisor", DepartmentId = 1, EmployeeId = "EMP002" },
        new User { Id = 3, Username = "officer1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("off123"), Role = "Officer", DepartmentId = 1, EmployeeId = "EMP003" },
        new User { Id = 4, Username = "junior1", PasswordHash = BCrypt.Net.BCrypt.HashPassword("jun123"), Role = "JuniorOfficer", DepartmentId = 1, EmployeeId = "EMP004" },
    };

    // Simulated list of valid employee IDs
    private readonly HashSet<string> _validEmployeeIds = new()
    {
        "EMP001", "EMP002", "EMP003", "EMP004", "EMP005", "EMP006", "EMP007", "EMP008", "EMP009", "EMP010"
    };

    public User GetByUsername(string username) =>
        _users.FirstOrDefault(u => u.Username == username);

    public User GetById(int id) =>
        _users.FirstOrDefault(u => u.Id == id);

    public IEnumerable<User> GetByDepartment(int departmentId) =>
        _users.Where(u => u.DepartmentId == departmentId);

    public IEnumerable<User> GetAll() =>
        _users.Select(u => new User
        {
            Id = u.Id,
            Username = u.Username,
            FullName = u.FullName,
            Email = u.Email,
            Role = u.Role,
            DepartmentId = u.DepartmentId,
            EmployeeId = u.EmployeeId
        });

    public User Create(UserCreateDto userDto)
    {
        if (!IsUsernameUnique(userDto.Username))
        {
            throw new InvalidOperationException("Username already exists");
        }

        var newUser = new User
        {
            Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1,
            Username = userDto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            FullName = userDto.FullName,
            Email = userDto.Email,
            Role = userDto.Role,
            DepartmentId = userDto.DepartmentId
        };

        _users.Add(newUser);
        return newUser;
    }

    public User CreateAccount(AccountCreateDto accountDto)
    {
        if (!IsUsernameUnique(accountDto.Username))
        {
            throw new InvalidOperationException("Username already exists");
        }

        if (!IsEmployeeIdValid(accountDto.EmployeeId))
        {
            throw new InvalidOperationException("Invalid Employee ID");
        }

        if (IsEmployeeIdRegistered(accountDto.EmployeeId))
        {
            throw new InvalidOperationException("Employee ID is already registered");
        }

        var newUser = new User
        {
            Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1,
            Username = accountDto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(accountDto.Password),
            FullName = accountDto.FullName,
            Email = accountDto.Email,
            Role = "Officer", // Default role for self-registered accounts
            DepartmentId = 1, // Default department
            EmployeeId = accountDto.EmployeeId
        };

        _users.Add(newUser);
        return newUser;
    }

    public bool IsUsernameUnique(string username)
    {
        return !_users.Any(u => u.Username == username);
    }

    public bool IsEmployeeIdValid(string employeeId)
    {
        return _validEmployeeIds.Contains(employeeId);
    }

    public bool IsEmployeeIdRegistered(string employeeId)
    {
        return _users.Any(u => u.EmployeeId == employeeId);
    }
}