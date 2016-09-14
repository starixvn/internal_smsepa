namespace SMS.EPA.Model.TargetEmployeeSetting
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Enitites;

    public class TargetDepartmentSettingModel
    {
        public IEnumerable<Department> Departments { get; set; }

        public IEnumerable<TargetDepartmentModel> TargetDepartments { get; set; }
    }
}