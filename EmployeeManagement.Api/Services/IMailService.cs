using EmployeeManagement.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EmployeeManagement.Api.Services
{
    public interface IMailService
    { 
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
