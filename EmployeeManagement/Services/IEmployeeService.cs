using EmployeeManagement.Models;

namespace EmployeeManagement.Services;

public interface IEmployeeService
{
    IEnumerable<Employee> GetAll();
    Employee? GetById(int id);
    Employee Create(Employee employee);
    bool Update(int id, Employee updatedEmployee);
    bool Delete(int id);
    bool Exists(int id);
}
