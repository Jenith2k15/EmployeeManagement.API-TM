using AutoMapper;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Models;
using EmployeeManagement.Api.Utilities;
using EmployeeManagement.Api.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Api.Services
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly EmployeeManagementContext dbContext;
        private readonly IMapper mapper;
        private readonly IMailService mailService;
        MailRequest mailRequest = new MailRequest();

        public EmployeeRepository(EmployeeManagementContext dbContext, IMapper mapper, IMailService mailService)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            this.mailService = mailService;
        }

        public async Task<IEnumerable<EmployeeDepartmentViewModel>> GetEmployees()
        {
            Task<IEnumerable<EmployeeDepartmentViewModel>> task = new Task<IEnumerable<EmployeeDepartmentViewModel>>(PerformJoin);
            task.Start();
            IEnumerable<EmployeeDepartmentViewModel> employees = await task;
            return employees;
        }

        public async Task<Employee> GetEmployee(int employeeId)
        {
            return await dbContext.Employees.Include(d=>d.Department).Include(m=>m.Manager)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
        }

        public async Task<Employee> AddEmployee(EmployeeViewModel employee)
        {
            try
            {
                Employee newEmployee = mapper.Map<Employee>(employee);
                var result = await dbContext.Employees.AddAsync(newEmployee);
                await dbContext.SaveChangesAsync();
                SendMail(result.Entity,RecordStatus.Created);
                return result.Entity;
            }

            catch(Exception ex)
            {
                throw ex;
            }
            
        }

        public async Task<Employee> UpdateEmployee(EmployeeViewModel employee)
        {
            try
            {
                var result = await dbContext.Employees.FirstOrDefaultAsync(emp => emp.EmployeeId == employee.EmployeeId);
            
                if(result != null)
                {
                    Employee employeeToUpdate = mapper.Map<Employee>(employee);
                    dbContext.Entry(result).State = EntityState.Detached;
                    dbContext.Entry(employeeToUpdate).State = EntityState.Modified;
                    await dbContext.SaveChangesAsync();
                    return employeeToUpdate;
                }
                return null;
            }

            catch(Exception ex)
            {
                throw ex;
            }
        }

        public async Task<Employee> DeleteEmployee(int employeeId)
        {
            var result = await dbContext.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
            if (result != null)
            {
                dbContext.Employees.Remove(result);
                await dbContext.SaveChangesAsync();
                SendMail(result,RecordStatus.Deleted);
                return result;
            }

            return null;
        }

        public async Task<Object> GetEmployeeStatusById(int? id)
        {
            var employeeDetail = await dbContext.Employees.FirstOrDefaultAsync(e => e.EmployeeId == id);
            if (dbContext.Employees.Where(x => x.ManagerId == id).Count() > 0)
            {
                return (from manager in dbContext.Employees.Where(x => x.ManagerId == id)
                        group manager by manager.ManagerId into g
                        select new
                        {
                            Status = employeeDetail.ManagerId == null ? "BigBoss" : g.Count() >= 1 ? "Manager" : "Associate",
                            NoOfEmployeesReporting = g.Count(),
                        });
            }

            return new { Status = "Associate",NoOfEmployeesReporting = 0  };
        }

        public async Task<int?> GetDepartmentId(string departmentName)
        {
            var department = await dbContext.Departments.FirstOrDefaultAsync(d => d.DepartmentName == departmentName);
            return department.DepartmentId;
        }

        public async Task<int?> GetManagerId(string managerName)
        {
            var manager = await dbContext.Employees.FirstOrDefaultAsync(e => e.FirstName == managerName);
            return manager.EmployeeId;
        }

        public async Task<IEnumerable<EmployeeDepartmentViewModel>> Search(int? employeeId, string department, string firstName, string lastName)
        {
            Task<IEnumerable<EmployeeDepartmentViewModel>> task = new Task<IEnumerable<EmployeeDepartmentViewModel>>(PerformJoin);
            task.Start();
            IEnumerable<EmployeeDepartmentViewModel> query = await task;

            if (employeeId != null && employeeId != 0)
            {
                query = query.Where(e => e.EmployeeId == employeeId);
            }

            if (!string.IsNullOrEmpty(department))
            {
                query = query.Where(e => e.Department.Contains(department));
            }

            if (!string.IsNullOrEmpty(firstName))
            {
                query = query.Where(e => e.FirstName.Contains(firstName));
            }

            if (!string.IsNullOrEmpty(lastName))
            {
                query = query.Where(e => e.LastName.Contains(lastName));
            }

            return query;
        }

        public IEnumerable<EmployeeDepartmentViewModel> PerformJoin()
        {
            var employees = dbContext.Employees.Include(x => x.Department).Select(emp => new EmployeeDepartmentViewModel
            {
                EmployeeId = emp.EmployeeId,
                FirstName = emp.FirstName,
                LastName = emp.LastName,
                Email = emp.EmailId,
                Department = emp.Department.DepartmentName,
                Manager = emp.Manager == null ?
                    "Super Boss" : emp.Manager.FirstName
            }).ToList();

            return employees;
        }

        public async void SendMail(Employee detail,RecordStatus recordStatus)
        {
            try
            {
                mailRequest.ToEmail = detail.EmailId;
                if(recordStatus == RecordStatus.Created)
                {
                    mailRequest.Subject = "Regarding registration";
                    mailRequest.Body = "Registration Successfull!";
                }
                
                else if(recordStatus == RecordStatus.Deleted)
                {
                    mailRequest.Subject = "Record deleted successfully";
                    mailRequest.Body = "Record deleted successfully!";
                }
                await mailService.SendEmailAsync(mailRequest);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}

//Employee newEmployee = await dbContext.Employees
//    .FirstOrDefaultAsync(e => e.EmployeeId == employee.EmployeeId);
//newEmployee.FirstName = employee.FirstName;
//newEmployee.LastName = employee.LastName;
//newEmployee.EmailId = employee.Email;
//newEmployee.DepartmentId = dbContext.Departments.FirstOrDefault(d => d.DepartmentName == employee.Department).DepartmentId;
//newEmployee.ManagerId = dbContext.Employees.FirstOrDefault(e => e.FirstName == employee.Manager).EmployeeId;

//Employee newEmployee = new Employee
//{
//    FirstName = employee.FirstName,
//    LastName = employee.LastName,
//    EmailId = employee.Email,
//    DepartmentId = dbContext.Departments.FirstOrDefault(d => d.DepartmentName == employee.Department).DepartmentId,
//    ManagerId = dbContext.Employees.FirstOrDefault(e => e.FirstName == employee.Manager).EmployeeId
//};

//var employees = from employee in dbContext.Employees.ToList()
//                join department in dbContext.Departments.ToList()
//                on employee.DepartmentId equals department.DepartmentId
//                join manager in dbContext.Employees.ToList()
//                on employee.ManagerId equals manager.EmployeeId
//                into eGroup
//                from e in eGroup.DefaultIfEmpty()
//                orderby employee.FirstName ascending
//                select new EmployeeDepartmentViewModel
//                {
//                    EmployeeId = employee.EmployeeId,
//                    FirstName = employee.FirstName,
//                    LastName = employee.LastName,
//                    Email = employee.EmailId,
//                    Department = department.DepartmentName,
//                    Manager = e == null ? "No Manager" : e.FirstName
//                };

//var employees = from employee in dbContext.Employees