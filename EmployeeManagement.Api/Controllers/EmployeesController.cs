using AutoMapper;
using EmployeeManagement.Api.Models;
using EmployeeManagement.Api.Services;
using EmployeeManagement.Api.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Api.Controllers
{
    /// <summary>
    /// Employees controller responsible for GET/POST/PUT/Delete for employee 
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeRepository employeeRepository;

        public EmployeesController(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository;
        }

        /// <summary>
        /// This GET method returns all employee
        /// </summary>
        /// <returns>An array of all employee detail</returns>
        [HttpGet]
        public async Task<ActionResult> GetEmployees()
        {
            try
            {
                return Ok(await employeeRepository.GetEmployees());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        /// <summary>
        /// This GET method returns an employee detail by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns>An instance of employee</returns>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Employee>> GetEmployee(int id)
        {
            try
            {
                var result = await employeeRepository.GetEmployee(id);

                if (result == null)
                {
                    return NotFound();
                }

                return result;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        /// <summary>
        /// This POST method is used to store new employee detail and send an email of created status.
        /// </summary>
        /// <param name="employee">New employee detail to store.</param>
        /// <returns>Status code 201 and newly created employee detail if employee is created successfully!.</returns>
        [HttpPost]
        public async Task<ActionResult<Employee>> CreateEmployee(EmployeeViewModel employee)
        {
            try
            {
                if (employee == null)
                    return BadRequest();

                var createdEmployee = await employeeRepository.AddEmployee(employee);

                //return Ok("Record created");
                return CreatedAtAction(nameof(GetEmployee),
                    new { id = createdEmployee.EmployeeId }, createdEmployee);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error creating new employee record");
            }
        }

        /// <summary>
        /// This PUT is used to update an employee by id
        /// </summary>
        /// <param name="id">EmployeeId to update.</param>
        /// <param name="employee">Employee detail to update.</param>
        /// <returns>Status code 200 and updated employee detail if employee is updated successfully!</returns>
        [HttpPut("{id:int}")]
        public async Task<ActionResult<Employee>> UpdateEmployee(int id, EmployeeViewModel employee)
        {
            try
            {
                if (id != employee.EmployeeId)
                    return BadRequest("Employee ID mismatch");

                var employeeToUpdate = await employeeRepository.GetEmployee(id);

                if (employeeToUpdate == null)
                    return NotFound($"Employee with Id = {id} not found");

               var updatedEmployee = await employeeRepository.UpdateEmployee(employee);

                return await employeeRepository.UpdateEmployee(employee);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error updating data");
            }
        }

        /// <summary>
        /// This Delete method is used to delete an employee by id and send an email of deleted status.
        /// </summary>
        /// <param name="id">EmployeeId to delete</param>
        /// <returns>An employee detail that is deleted.</returns>
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<Employee>> DeleteEmployee(int id)
        {
            try
            {
                var employeeToDelete = await employeeRepository.GetEmployee(id);

                if (employeeToDelete == null)
                {
                    return NotFound($"Employee with Id = {id} not found");
                }

                return await employeeRepository.DeleteEmployee(id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error deleting data");
            }
        }

        /// <summary>
        /// This GET method is used to get employee status like an employee is BigBoss/Manager/Associate
        /// </summary>
        /// <param name="id">EmployeeId to get the status of employee</param>
        /// <returns>Status of employee by supplied EmployeeId</returns>
        [HttpGet("[action]")]
        public async Task<object> GetEmployeeStatus(int id)
        {
            try
            {
                var result = await employeeRepository.GetEmployeeStatusById(id);
                if (result == null)
                {
                    return NotFound();
                }
                return result;
            }

            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                   "Error retrieving data from the database");
            }

        }

        /// <summary>
        /// This GET method is used to search employee by EmployeeId/Department/FirstName/LastName.
        /// </summary>
        /// <param name="employeeId">EmployeeId to search</param>
        /// <param name="department">Department name to search</param>
        /// <param name="firstName">FirstName to search</param>
        /// <param name="LastName">LastName to search</param>
        /// <returns>Search result</returns>
        [HttpGet("[action]")]
        public async Task<ActionResult<IEnumerable<EmployeeDepartmentViewModel>>> Search(int? employeeId, string department,string firstName,string LastName)
        {
            try
            {
                var result = await employeeRepository.Search(employeeId, department, firstName, LastName);

                if (result.Any())
                {
                    return Ok(result);
                }

                return NotFound();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                "Error retrieving data from the database");
            }
        }
    }
}
