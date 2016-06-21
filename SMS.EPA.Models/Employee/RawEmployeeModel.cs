namespace SMS.EPA.Model.Employee
{
    using System.Collections.Generic;

    public class RawEmployeeModel
    {
        public IEnumerable<EmployeeModel> Employees { get; set; }

        public IEnumerable<RawEmployeeRoleModel> Roles { get; set; }
    }
}