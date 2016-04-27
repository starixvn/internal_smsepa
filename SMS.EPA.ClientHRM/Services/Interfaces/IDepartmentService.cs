using SMS.EPA.ClientHRM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMS.EPA.ClientHRM.Services
{
    public interface IDepartmentService
    {
        IEnumerable<Department> GetDepartments(DateTime? fromDate);

        DateTime GetMaxModifiedDate(TokenAccess token);

        CommandResult Update(IEnumerable<Department> departments, TokenAccess token);
    }
}
