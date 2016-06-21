namespace SMS.EPA.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Employee;

    public class TargetEmployeeSettingModel
    {
        public IEnumerable<EmployeeModel> Employees { get; set; }

        public IEnumerable<TargetEmployeeModel> TargetEmployees { get; set; }
    }
}