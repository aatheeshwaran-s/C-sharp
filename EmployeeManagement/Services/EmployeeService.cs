using EmployeeManagement.Models;

namespace EmployeeManagement.Services;

public class EmployeeService : IEmployeeService
{
    private readonly List<Employee> _employees = new()
    {
        new Employee { Id = 101, Name = "Alice Johnson", Department = "IT", Salary = 75000 },
        new Employee { Id = 102, Name = "Bob Smith", Department = "HR", Salary = 60000 }
    };

    public Task<IEnumerable<Employee>> GetAllAsync()
    {
        return Task.FromResult<IEnumerable<Employee>>(_employees.ToList());
    }

    public Task<Employee?> GetByIdAsync(int id)
    {
        var emp = _employees.FirstOrDefault(e => e.Id == id);
        return Task.FromResult(emp);
    }

    public Task<Employee> CreateAsync(Employee employee)
    {
        if (employee.Salary < 0)
        {
            throw new ArgumentException("Salary cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(employee.Name))
        {
            throw new ArgumentException("Employee name is required.");
        }

        if (_employees.Any(e => e.Id == employee.Id))
        {
            throw new InvalidOperationException($"Employee with ID {employee.Id} already exists.");
        }

        _employees.Add(employee);
        return Task.FromResult(employee);
    }

    public Task<bool> UpdateAsync(int id, Employee updatedEmployee)
    {
        var existing = _employees.FirstOrDefault(e => e.Id == id);
        if (existing == null)
        {
            return Task.FromResult(false);
        }

        if (updatedEmployee.Salary < 0)
        {
            throw new ArgumentException("Salary cannot be negative.");
        }

        existing.Name = updatedEmployee.Name;
        existing.Department = updatedEmployee.Department;
        existing.Salary = updatedEmployee.Salary;
        return Task.FromResult(true);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var existing = _employees.FirstOrDefault(e => e.Id == id);
        if (existing == null)
        {
            return Task.FromResult(false);
        }

        _employees.Remove(existing);
        return Task.FromResult(true);
    }

    public Task<bool> ExistsAsync(int id)
    {
        return Task.FromResult(_employees.Any(e => e.Id == id));
    }
}
