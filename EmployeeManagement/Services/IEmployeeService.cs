using EmployeeManagement.Models;

namespace EmployeeManagement.Services;

public interface IEmployeeService
{
    Task<IEnumerable<Employee>> GetAllAsync();
    Task<Employee?> GetByIdAsync(int id);
    Task<Employee> CreateAsync(Employee employee);
    Task<bool> UpdateAsync(int id, Employee updatedEmployee);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}
