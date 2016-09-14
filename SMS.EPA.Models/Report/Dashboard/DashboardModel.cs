namespace SMS.EPA.Model.Report.Dashboard
{
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Model.Enitites;
    using System.Collections.Generic;

    public class DashboardModel
    {
        public Employee Employee { get; set; }

        public IEnumerable<EPAModel> MyEPAs { get; set; }

        public IEnumerable<EPAModel> MonthlyEPAs { get; set; }
    }
}