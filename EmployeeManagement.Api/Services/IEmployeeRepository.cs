using EmployeeManagement.Api.Models;
using EmployeeManagement.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Api.Services
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<EmployeeDepartmentViewModel>> GetEmployees();
        Task<Employee> GetEmployee(int employeeId);
        Task<Employee> AddEmployee(EmployeeViewModel employee);
        Task<Employee> UpdateEmployee(EmployeeViewModel employee);
        Task<Employee> DeleteEmployee(int employeeId);
        Task<object> GetEmployeeStatusById(int? id);
        Task<IEnumerable<EmployeeDepartmentViewModel>> Search(int? employeeId, string department, string firstName, string LastName);
    }
}
