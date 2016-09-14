namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Enitites;

    public class EmployeeFlowSettingModel
    {
        public IEnumerable<Employee> DefaultFlowSettings { get; set; }

        public IEnumerable<Employee> PersonalFlowSettings { get; set; }
    }
}