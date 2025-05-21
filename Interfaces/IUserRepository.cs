using Helpdesk.Api.Models;

namespace Helpdesk.Api.Interfaces;

public interface IUserRepository
{
    User GetByUsername(string username);
    User GetById(int id);
    IEnumerable<User> GetByDepartment(int departmentId);
    IEnumerable<User> GetAll();
    User Create(UserCreateDto userDto);
    bool IsUsernameUnique(string username);
    User CreateAccount(AccountCreateDto accountDto);
    bool IsEmployeeIdValid(string employeeId);
    bool IsEmployeeIdRegistered(string employeeId);
}