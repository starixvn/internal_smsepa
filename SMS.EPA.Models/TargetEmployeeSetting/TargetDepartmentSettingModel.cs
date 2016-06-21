namespace SMS.EPA.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Department;

    public class TargetDepartmentSettingModel
    {
        public IEnumerable<DepartmentModel> Departments { get; set; }

        public IEnumerable<TargetDepartmentModel> TargetDepartments { get; set; }
    }
}