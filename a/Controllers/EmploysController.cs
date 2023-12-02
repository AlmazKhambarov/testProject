using a.Models;
using a.FluentValidation;
using a.Services.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using a.Data;
using System.Collections.Generic;

namespace a.Controllers
{
    public class EmploysController : Controller
    {
        private readonly IEmployeesRepository _employeesRepository;
        private readonly ApplicationDbContext _applicationDbContext;

        public EmploysController(IEmployeesRepository employeesRepository, ApplicationDbContext applicationDbContext)
        {
            
            _employeesRepository = employeesRepository;
            _applicationDbContext = applicationDbContext;
        }
        public async Task<IActionResult> Index()
        {
            var products = await _employeesRepository.GetAllEmployees();
            return View(products);
        }


        public async Task<IActionResult> Details(int id)
        {
            try
            {
                if (id == null) return NotFound();

                var product = await _employeesRepository.GetEmployeeIdAsync(id);
                if (product == null) return NotFound();

                return View(product);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "NotFoundPage");
            }

        }
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(Employeese employee)
        {
            var validationResult = await new EmploysModelValidation().ValidateAsync(employee);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return View(employee);
            }
            if (!ModelState.IsValid) return View(employee);
            Employeese employee1 = new Employeese
            {
                UserName = employee.UserName,
                Department = employee.Department,
                BirthDate = employee.BirthDate?.ToUniversalTime(),
                DateOfEmployment = employee.DateOfEmployment?.ToUniversalTime(),
                Wage = employee.Wage
            };
            await _employeesRepository.CreateEmployeeAsync(employee1);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                if (id == null) return NotFound();
                var product = await _employeesRepository.GetEmployeeIdAsync(id);
                if (product == null) return NotFound();
                return View(product);
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "NotFoundPage");
            }
        
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employeese employee)
        {
            var validationResult = await new EmploysModelValidation().ValidateAsync(employee);
            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }

                return View(employee);
            }
            if (!ModelState.IsValid) return View(employee);

            try
            {
                Employeese employee1 = new Employeese
                {
                    Id = employee.Id,
                    UserName = employee.UserName,
                    Department = employee.Department,
                    BirthDate = employee.BirthDate?.ToUniversalTime(),
                    DateOfEmployment = employee.DateOfEmployment?.ToUniversalTime(),
                    Wage = employee.Wage
                };
                var newProduct = await _employeesRepository.UpdateEmployeeAsync(employee1);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (_employeesRepository.GetEmployeeIdAsync(employee.Id) == null)
                    return NotFound();
                else
                    throw;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (id == null) return NotFound();

            var product = await _employeesRepository.GetEmployeeIdAsync(id);

            if (product == null) return NotFound();

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var product = await _employeesRepository.DeleteEmployeeAsync(id);
                if (product == null) return NotFound();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "NotFoundPage");
            }

        }

        public async Task<IActionResult> EmploysDetails(string filter, int age)
        {
            // Обработка запроса в зависимости от значения параметра filter
            switch (filter)
            {
                case "all":
                    var allEmployees = await _applicationDbContext.Employee.OrderBy(p => p.Id).ToListAsync(); // Получите всех сотрудников из базы данных
                    return View("Index", allEmployees); // Передача списка сотрудников в представление Index.cshtml
                case "highSalary":
                    var highSalaryEmployees = await  _applicationDbContext.Employee.Where(e => e.Wage > 10000).ToListAsync(); // Получите сотрудников с высокой зарплатой
                    return View("Index", highSalaryEmployees);
                case "olderThan70":
                    var s = await _applicationDbContext.Employee.ToListAsync();
                    var olderThan70Employees = s.Where(e => CalculateAge(e.BirthDate) > age).ToList();

                    if (olderThan70Employees.Any())
                    {
                        return View("Index", olderThan70Employees);
                    }
                    else
                    {
                        TempData["Message"] = "No employees older than 70 found.";
                        return RedirectToAction("Index");
                    }
                case "updateSalary":
                    var employeesToUpdate = await _applicationDbContext.Employee.Where(e => e.Wage < 15000).ToListAsync(); // Получите сотрудников с зарплатой меньше 15000
                    foreach (var employee in employeesToUpdate)
                    {
                        employee.Wage = 15000; // Обновите зарплату сотрудников
                    }
                    _applicationDbContext.SaveChanges(); // Сохраните изменения в базе данных
                    return RedirectToAction("Index"); // Перенаправление на страницу со всеми сотрудниками
                default:
                    return View("Index", await _applicationDbContext.Employee.OrderBy(p => p.Id).ToListAsync()); // По умолчанию, показать всех сотрудников
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
}

