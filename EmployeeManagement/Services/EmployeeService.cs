using EmployeeManagement.Models;

namespace EmployeeManagement.Services;

public class EmployeeService : IEmployeeService
{
    private readonly List<Employee> _employees = new()
    {
        new Employee { Id = 101, Name = "Alice Johnson", Department = "IT", Salary = 75000 },
        new Employee { Id = 102, Name = "Bob Smith", Department = "HR", Salary = 60000 }
    };

    public IEnumerable<Employee> GetAll()
    {
        return _employees.ToList();
    }

    public Employee? GetById(int id)
    {
        return _employees.FirstOrDefault(e => e.Id == id);
    }

    public Employee Create(Employee employee)
    {
        if (employee.Salary < 0)
        {
            throw new ArgumentException("Salary cannot be negative.");
        }

        if (string.IsNullOrWhiteSpace(employee.Name))
        {
            throw new ArgumentException("Employee name is required.");
        }

        if (Exists(employee.Id))
        {
            throw new InvalidOperationException($"Employee with ID {employee.Id} already exists.");
        }

        _employees.Add(employee);
        return employee;
    }

    public bool Update(int id, Employee updatedEmployee)
    {
        var existing = GetById(id);
        if (existing == null)
        {
            return false;
        }

        if (updatedEmployee.Salary < 0)
        {
            throw new ArgumentException("Salary cannot be negative.");
        }

        existing.Name = updatedEmployee.Name;
        existing.Department = updatedEmployee.Department;
        existing.Salary = updatedEmployee.Salary;
        return true;
    }

    public bool Delete(int id)
    {
        var existing = GetById(id);
        if (existing == null)
        {
            return false;
        }

        _employees.Remove(existing);
        return true;
    }

    public bool Exists(int id)
    {
        return _employees.Any(e => e.Id == id);
    }
}
