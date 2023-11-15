using a.Data;
using a.Models;
using a.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace a.Services.Repository;

public class EmployeesRepository: IEmployeesRepository
{
    private readonly ApplicationDbContext _applicationDbContext;

    public EmployeesRepository(ApplicationDbContext applicationDbContext)
    {
        _applicationDbContext = applicationDbContext;
    }
    public  async Task<IEnumerable<Employeese>> GetAllEmployees() => await _applicationDbContext.Employee.OrderBy(p => p.Id).ToListAsync();

    public async Task<Employeese> GetEmployeeIdAsync(int id)
    {
        var currentEmployee = await _applicationDbContext.Employee.FirstOrDefaultAsync(x => x.Id == id);
        return currentEmployee ?? throw new Exception("Employee not found");
    }

    public async   Task<Employeese> CreateEmployeeAsync(Employeese entity)
    {
        await _applicationDbContext.Employee.AddAsync(entity);
        await _applicationDbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<Employeese> UpdateEmployeeAsync(Employeese entity)
    {
        var currentEmployee = await _applicationDbContext.Employee.FirstOrDefaultAsync(x => x.Id == entity.Id);
        if (currentEmployee == null) throw new Exception("Employee not found");

        currentEmployee.UserName = entity.UserName;
        currentEmployee.Department = entity.Department;
        currentEmployee.BirthDate = entity.BirthDate?.ToUniversalTime();
        currentEmployee.DateOfEmployment = entity.DateOfEmployment?.ToUniversalTime();
        currentEmployee.Wage = entity.Wage;
        await _applicationDbContext.SaveChangesAsync();
        return entity;
    }

    public async Task<Employeese> DeleteEmployeeAsync(int Id)
    {
        var currentEmployee = await _applicationDbContext.Employee.FirstOrDefaultAsync(x => x.Id == Id);
        if (currentEmployee == null) throw new Exception("Employee not found");
        _applicationDbContext.Employee.Remove(currentEmployee);
        _applicationDbContext.SaveChanges();
        return currentEmployee;
    }
}