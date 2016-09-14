namespace SMS.EPA.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Enitites;

    public class TargetEmployeeSettingModel
    {
        public IEnumerable<Employee> Employees { get; set; }

        public IEnumerable<TargetEmployeeModel> TargetEmployees { get; set; }
    }
}