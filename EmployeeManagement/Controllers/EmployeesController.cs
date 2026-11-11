using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Models;

namespace EmployeeManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    // In-memory list simulating a database table for Employees
    private static readonly List<Employee> _employees = new()
    {
        new Employee { Id = 101, Name = "Alice Johnson", Department = "IT", Salary = 75000 },
        new Employee { Id = 102, Name = "Bob Smith", Department = "HR", Salary = 60000 }
    };

    // 1. READ ALL: GET http://localhost:5000/api/employees
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_employees);
    }

    // 2. READ ONE: GET http://localhost:5000/api/employees/101
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        if (employee == null)
        {
            return NotFound($"Employee with ID {id} not found."); // HTTP 404
        }
        return Ok(employee); // HTTP 200
    }

    // 3. CREATE: POST http://localhost:5000/api/employees
    [HttpPost]
    public IActionResult Create([FromBody] Employee newEmployee)
    {
        // Simple validation: check if ID already exists
        if (_employees.Any(e => e.Id == newEmployee.Id))
        {
            return BadRequest($"Employee with ID {newEmployee.Id} already exists."); // HTTP 400
        }

        _employees.Add(newEmployee);
        // Returns HTTP 201 Created with the location of the new resource
        return CreatedAtAction(nameof(GetById), new { id = newEmployee.Id }, newEmployee);
    }

    // 4. UPDATE: PUT http://localhost:5000/api/employees/101
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Employee updatedEmployee)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        if (employee == null)
        {
            return NotFound($"Employee with ID {id} not found."); // HTTP 404
        }

        employee.Name = updatedEmployee.Name;
        employee.Department = updatedEmployee.Department;
        employee.Salary = updatedEmployee.Salary;

        return Ok(employee); // HTTP 200
    }

    // 5. DELETE: DELETE http://localhost:5000/api/employees/101
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var employee = _employees.FirstOrDefault(e => e.Id == id);
        if (employee == null)
        {
            return NotFound($"Employee with ID {id} not found."); // HTTP 404
        }

        _employees.Remove(employee);
        return NoContent(); // HTTP 204 (Success, nothing to display)
    }
}
