public interface IUserRepository
{
    User GetByUsername(string username);
    User GetById(int id);
    IEnumerable<User> GetByDepartment(int departmentId);
}