using a.Models;

namespace a.Services.Interface;

public interface IEmployeesRepository
{
    Task<Employeese> GetEmployeeIdAsync(int id);
    Task<IEnumerable<Employeese>> GetAllEmployees();
    Task<Employeese> CreateEmployeeAsync(Employeese entity);
    Task<Employeese> UpdateEmployeeAsync(Employeese entity);
    Task<Employeese> DeleteEmployeeAsync(int Id);
}