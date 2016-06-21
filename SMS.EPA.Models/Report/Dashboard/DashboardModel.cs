namespace SMS.EPA.Model.Report.Dashboard
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.EmployeeEPA;

    public class DashboardModel
    {
        public EmployeeModel Employee { get; set; }

        public IEnumerable<EPAModel> MyEPAs { get; set; }

        public IEnumerable<EPAModel> MonthlyEPAs { get; set; }
    }
}