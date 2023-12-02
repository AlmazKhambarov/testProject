using a.Data;
using a.FluentValidation;
using a.Models;
using a.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeesRepository _employeesRepository;
    private readonly ApplicationDbContext _applicationDbContext;

    public EmployeesController(IEmployeesRepository employeesRepository, ApplicationDbContext applicationDbContext)
    {
        _employeesRepository = employeesRepository;
        _applicationDbContext = applicationDbContext;
    }
    [HttpGet]
    public async Task<IActionResult> GetAllEmployes() => Ok(await _employeesRepository.GetAllEmployees());
    
    [HttpPost("Create")]
    public async Task<IActionResult> Create(Employeese employee)
    {
        var validationResult = await new EmploysModelValidation().ValidateAsync(employee);

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return BadRequest(ModelState);
        }

        Employeese newEmployee = new Employeese
        {
            UserName = employee.UserName,
            Department = employee.Department,
            BirthDate = employee.BirthDate?.ToUniversalTime(),
            DateOfEmployment = employee.DateOfEmployment?.ToUniversalTime(),
            Wage = employee.Wage
        };

        await _employeesRepository.CreateEmployeeAsync(newEmployee);

        return Ok("Employee created successfully");
    }

    [HttpGet("GetById/{id}")]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            if (id == null) return NotFound();

            var employee = await _employeesRepository.GetEmployeeIdAsync(id);

            if (employee == null) return NotFound();

            return Ok(employee);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPut("Edit")]
    public async Task<IActionResult> Edit(Employeese employee)
    {
        var validationResult = await new EmploysModelValidation().ValidateAsync(employee);

        if (!validationResult.IsValid)
        {
            foreach (var error in validationResult.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return BadRequest(ModelState);
        }

        try
        {
            Employeese updatedEmployee = new Employeese
            {
                Id = employee.Id,
                UserName = employee.UserName,
                Department = employee.Department,
                BirthDate = employee.BirthDate?.ToUniversalTime(),
                DateOfEmployment = employee.DateOfEmployment?.ToUniversalTime(),
                Wage = employee.Wage
            };

            var newEmployee = await _employeesRepository.UpdateEmployeeAsync(updatedEmployee);

            return Ok("Employee updated successfully");
        }
        catch (DbUpdateConcurrencyException)
        {
            if (_employeesRepository.GetEmployeeIdAsync(employee.Id) == null)
                return NotFound();
            else
                return StatusCode(500, "Internal server error");
        }
    }



    [HttpDelete("Delete/{id}")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var employee = await _employeesRepository.DeleteEmployeeAsync(id);

            if (employee == null) return NotFound();

            return Ok("Employee deleted successfully");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("EmploysDetails")]
    public async Task<IActionResult> EmploysDetails(string filter)
    {
        switch (filter)
        {
            case "all":
                var allEmployees = await _applicationDbContext.Employee.OrderBy(p => p.Id).ToListAsync();
                return Ok(allEmployees);
            case "highSalary":
                var highSalaryEmployees = await _applicationDbContext.Employee.Where(e => e.Wage > 10000).ToListAsync();
                return Ok(highSalaryEmployees);
            case "olderThan70":
                var employees = await _applicationDbContext.Employee.ToListAsync();
                var olderThan70Employees = employees.Where(e => CalculateAge(e.BirthDate) > 70).ToList();

                if (olderThan70Employees.Any())
                {
                    return Ok(olderThan70Employees);
                }
                else
                {
                    return NotFound("No employees older than 70 found.");
                }
            case "updateSalary":
                var employeesToUpdate = await _applicationDbContext.Employee.Where(e => e.Wage < 15000).ToListAsync();
                foreach (var employee in employeesToUpdate)
                {
                    employee.Wage = 15000;
                }
                _applicationDbContext.SaveChanges();
                return Ok("Salaries updated successfully.");
            default:
                var defaultEmployees = await _applicationDbContext.Employee.OrderBy(p => p.Id).ToListAsync();
                return Ok(defaultEmployees);
        }
    }

    private int CalculateAge(DateTime? dateOfBirth)
    {
        if (dateOfBirth.HasValue)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Value.Year;

            if (today.Month < dateOfBirth.Value.Month || (today.Month == dateOfBirth.Value.Month && today.Day < dateOfBirth.Value.Day))
            {
                age--;
            }

            return age;
        }

        return 0;
    }
}
