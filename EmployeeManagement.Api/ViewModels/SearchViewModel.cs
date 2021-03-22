using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Api.ViewModels
{
    public class SearchViewModel
    {
        public int EmployeeId { get; set; }
        public string Department { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
    }
}
