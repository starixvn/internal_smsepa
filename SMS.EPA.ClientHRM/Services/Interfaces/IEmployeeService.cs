using SMS.EPA.ClientHRM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.EPA.ClientHRM.Services
{
    public interface IEmployeeService
    {
        IEnumerable<Employee> GetEmployees(DateTime? fromDate);

        DateTime GetMaxModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Employee> departments, TokenAccess token);
    }
}
