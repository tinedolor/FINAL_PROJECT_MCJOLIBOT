using Helpdesk.Api.Data;
using Helpdesk.Api.Interfaces;
using Helpdesk.Api.Models;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace Helpdesk.Api.Repositories;

public class DatabaseUserRepository : IUserRepository
{
    private readonly HelpdeskDbContext _context;

    public DatabaseUserRepository(HelpdeskDbContext context)
    {
        _context = context;
    }

    public User GetByUsername(string username)
    {
        return _context.Users
            .FirstOrDefault(u => u.Username == username);
    }

    public User GetById(int id)
    {
        return _context.Users
            .FirstOrDefault(u => u.Id == id);
    }

    public IEnumerable<User> GetByDepartment(int departmentId)
    {
        return _context.Users
            .Where(u => u.DepartmentId == departmentId)
            .ToList();
    }

    public IEnumerable<User> GetAll()
    {
        return _context.Users
            .Select(u => new User
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role,
                DepartmentId = u.DepartmentId,
                EmployeeId = u.EmployeeId
            })
            .ToList();
    }

    public User Create(UserCreateDto userDto)
    {
        if (!IsUsernameUnique(userDto.Username))
        {
            throw new InvalidOperationException("Username already exists");
        }

        var newUser = new User
        {
            Username = userDto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(userDto.Password),
            FullName = userDto.FullName,
            Email = userDto.Email,
            Role = userDto.Role,
            DepartmentId = userDto.DepartmentId
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();
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
            Username = accountDto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(accountDto.Password),
            FullName = accountDto.FullName,
            Email = accountDto.Email,
            Role = "Officer", // Default role for self-registered accounts
            DepartmentId = 1, // Default department
            EmployeeId = accountDto.EmployeeId
        };

        _context.Users.Add(newUser);
        _context.SaveChanges();
        return newUser;
    }

    public bool IsUsernameUnique(string username)
    {
        return !_context.Users.Any(u => u.Username == username);
    }

    public bool IsEmployeeIdValid(string employeeId)
    {
        // EmployeeId in your table is an int, so parse it
        if (!int.TryParse(employeeId, out int empId))
            return false;
        return _context.Employees.Any(e => e.EmployeeId == empId);
    }

    public bool IsEmployeeIdRegistered(string employeeId)
    {
        return _context.Users.Any(u => u.EmployeeId == employeeId);
    }
} 