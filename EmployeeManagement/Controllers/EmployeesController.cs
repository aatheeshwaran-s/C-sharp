using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Models;
using EmployeeManagement.Services;

namespace EmployeeManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeesController> _logger;

    // Day 07: Constructor Injection (ASP.NET Core DI provides these instances automatically)
    public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
    {
        _employeeService = employeeService;
        _logger = logger;
    }

    // 1. GET ALL: GET /api/employees
    [HttpGet]
    public IActionResult GetAll()
    {
        _logger.LogInformation("Fetching all employees.");
        var employees = _employeeService.GetAll();
        return Ok(employees);
    }

    // 2. GET BY ID: GET /api/employees/{id}
    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        _logger.LogInformation("Fetching employee with ID {Id}.", id);
        var employee = _employeeService.GetById(id);
        if (employee == null)
        {
            _logger.LogWarning("Employee with ID {Id} not found.", id);
            return NotFound($"Employee with ID {id} not found.");
        }
        return Ok(employee);
    }

    // 3. CREATE: POST /api/employees
    [HttpPost]
    public IActionResult Create([FromBody] Employee newEmployee)
    {
        _logger.LogInformation("Attempting to create employee with ID {Id}.", newEmployee.Id);
        try
        {
            var created = _employeeService.Create(newEmployee);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation failure while creating employee.");
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflict error: duplicate employee ID.");
            return Conflict(ex.Message);
        }
    }

    // 4. UPDATE: PUT /api/employees/{id}
    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] Employee updatedEmployee)
    {
        _logger.LogInformation("Updating employee with ID {Id}.", id);
        try
        {
            var success = _employeeService.Update(id, updatedEmployee);
            if (!success)
            {
                return NotFound($"Employee with ID {id} not found.");
            }
            return Ok(updatedEmployee);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // 5. DELETE: DELETE /api/employees/{id}
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        _logger.LogInformation("Deleting employee with ID {Id}.", id);
        var success = _employeeService.Delete(id);
        if (!success)
        {
            return NotFound($"Employee with ID {id} not found.");
        }
        return NoContent();
    }
}
