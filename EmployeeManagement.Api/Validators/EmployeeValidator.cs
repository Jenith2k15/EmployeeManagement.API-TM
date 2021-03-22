using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.ViewModels;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Api.Validators
{
    public class EmployeeValidator : AbstractValidator<EmployeeViewModel>
    {
        private readonly EmployeeManagementContext dbContext;

        public EmployeeValidator(EmployeeManagementContext dbContext)
        {
            RuleFor(emp => emp.FirstName).NotEmpty()
                                         .WithMessage("{PropertyName} should be not empty.")
                                         .Must(IsValid)
                                         .WithMessage("{PropertyName} should be all letters.");
            
            RuleFor(emp => emp.LastName).NotEmpty()
                                        .WithMessage("{PropertyName} should be not empty.")
                                        .Must(IsValid)
                                        .WithMessage("{PropertyName} should be all letters.");

            RuleFor(emp => emp.EmailId).NotEmpty()
                                       .WithMessage("{PropertyName} should be not empty.")
                                       .EmailAddress()
                                       .WithMessage("{PropertyName} should be in valid email format."); ;

            RuleFor(emp => emp.DepartmentId).NotEmpty()
                                          .WithMessage("{PropertyName} should be not empty.");
            
            RuleFor(emp => emp.ManagerId).NotEmpty()
                                       .WithMessage("{PropertyName} should be not empty.")
                                       .Must(IsThereManagerNullAlready)
                                       .WithMessage("{PropertyName} cannot be null. Already there is a manager with null");

            this.dbContext = dbContext;
        }

        private bool IsValid(string firstName)
        {
            return firstName.All(Char.IsLetter);
        }

        private bool IsThereManagerNullAlready(int? mangerId)
        {
            var result = dbContext.Employees.FirstOrDefault(emp => emp.ManagerId == null);
            if (result != null)
            {
                return true;
            } 
            return false;
        }
    }
}
