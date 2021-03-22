using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Api.ViewModels
{
    public class EmployeeDepartmentViewModel
    {
        public int? EmployeeId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        //public int? DepartmentId { get { return 1; } }
        public string Manager { get; set; }
        //public int? ManagerId { get { return 1; } }
    }
}
