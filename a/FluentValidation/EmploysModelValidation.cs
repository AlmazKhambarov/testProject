using a.Models;
using FluentValidation;

namespace a.FluentValidation;

public class EmploysModelValidation: AbstractValidator<Employeese>
{
    public EmploysModelValidation()
    {
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("UserName is required.")
            .Must(userName => userName == null || char.IsUpper(userName, 0))
            .WithMessage("UserName should start with an uppercase letter.");

        RuleFor(x => x.Department)
            .NotEmpty().WithMessage("Department is required.")
            .Must(department => department == null || char.IsUpper(department, 0))
            .WithMessage("Department should start with an uppercase letter.");
        RuleFor(x => x.BirthDate)
            .NotEmpty().WithMessage("BirthDate is required.")
            //.Must(birthDate => birthDate == default || birthDate.Year <= 2005)
            .WithMessage("Year of birth must be 2005 or earlier.");
        RuleFor(x => x.DateOfEmployment)
            .NotEmpty().WithMessage("Date of Employment is required.")
            .Must(dateOfEmployment => dateOfEmployment == null || dateOfEmployment <= DateTime.Today)
            .WithMessage("Date of Employment must be today or a past date.");

        RuleFor(x => x.Wage)
            .NotEmpty().WithMessage("Wage is required.")
            .Must(x => x >= 1000 && x <= 15000).WithMessage("Wage must be between 1000 and 15000.");
    }
}