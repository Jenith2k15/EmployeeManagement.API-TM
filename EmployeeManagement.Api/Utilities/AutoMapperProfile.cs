using AutoMapper;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Models;
using EmployeeManagement.Api.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Api.Utilities
{
    public class AutoMapperProfile : Profile
    {
        EmployeeManagementContext dbContext = new EmployeeManagementContext();
        public AutoMapperProfile()
        {
            CreateMap<EmployeeViewModel, Employee>();
            CreateMap<Employee, EmployeeViewModel>();
        }
    }
}
