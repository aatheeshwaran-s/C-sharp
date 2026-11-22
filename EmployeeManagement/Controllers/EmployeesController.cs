using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.Models;
using EmployeeManagement.Services;
using MongoDB.Driver;

namespace EmployeeManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeesController> _logger;

    public EmployeesController(IEmployeeService employeeService, ILogger<EmployeesController> logger)
    {
        _employeeService = employeeService;
        _logger = logger;
    }

    // 1. GET ALL: GET /api/employees
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        _logger.LogInformation("Fetching all employees from database.");
        try
        {
            var employees = await _employeeService.GetAllAsync();
            return Ok(employees);
        }
        catch (TimeoutException ex)
        {
            _logger.LogError(ex, "MongoDB connection timeout.");
            return StatusCode(503, new { error = "Cannot connect to MongoDB. Please ensure your MongoDB service is running on mongodb://localhost:27017." });
        }
        catch (MongoConnectionException ex)
        {
            _logger.LogError(ex, "MongoDB connection refused.");
            return StatusCode(503, new { error = "Cannot connect to MongoDB. Please ensure your MongoDB service is running on mongodb://localhost:27017." });
        }
    }

    // 2. GET BY ID: GET /api/employees/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        _logger.LogInformation("Fetching employee with ID {Id}.", id);
        try
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {Id} not found.", id);
                return NotFound(new { message = $"Employee with ID {id} not found." });
            }
            return Ok(employee);
        }
        catch (Exception ex) when (ex is TimeoutException or MongoConnectionException)
        {
            return StatusCode(503, new { error = "Cannot connect to MongoDB. Please ensure your MongoDB service is running on mongodb://localhost:27017." });
        }
    }

    // 3. CREATE: POST /api/employees
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Employee newEmployee)
    {
        _logger.LogInformation("Creating employee with ID {Id}.", newEmployee.Id);
        try
        {
            var created = await _employeeService.CreateAsync(newEmployee);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error creating employee.");
            return BadRequest(new { error = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Conflict error creating employee.");
            return Conflict(new { error = ex.Message });
        }
        catch (Exception ex) when (ex is TimeoutException or MongoConnectionException)
        {
            _logger.LogError(ex, "MongoDB connection failed.");
            return StatusCode(503, new { error = "Cannot connect to MongoDB. Please ensure your MongoDB service is running on mongodb://localhost:27017." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error communicating with database.");
            return StatusCode(500, new { error = "Internal server error. Ensure database is running." });
        }
    }

    // 4. UPDATE: PUT /api/employees/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Employee updatedEmployee)
    {
        _logger.LogInformation("Updating employee with ID {Id}.", id);
        try
        {
            var success = await _employeeService.UpdateAsync(id, updatedEmployee);
            if (!success)
            {
                return NotFound(new { message = $"Employee with ID {id} not found." });
            }
            return Ok(updatedEmployee);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex) when (ex is TimeoutException or MongoConnectionException)
        {
            return StatusCode(503, new { error = "Cannot connect to MongoDB. Please ensure your MongoDB service is running on mongodb://localhost:27017." });
        }
    }

    // 5. DELETE: DELETE /api/employees/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting employee with ID {Id}.", id);
        try
        {
            var success = await _employeeService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(new { message = $"Employee with ID {id} not found." });
            }
            return NoContent();
        }
        catch (Exception ex) when (ex is TimeoutException or MongoConnectionException)
        {
            return StatusCode(503, new { error = "Cannot connect to MongoDB. Please ensure your MongoDB service is running on mongodb://localhost:27017." });
        }
    }
}
