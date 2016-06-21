namespace SMS.EPA.Model.FlowManagement
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Employee;

    public class EmployeeFlowSettingModel
    {
        public IEnumerable<EmployeeModel> DefaultFlowSettings { get; set; }

        public IEnumerable<EmployeeModel> PersonalFlowSettings { get; set; }
    }
}