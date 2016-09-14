namespace SMS.EPA.Model.Report.Dashboard
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.EmployeeEPA;

    public class DashboardModel
    {
        public Employee Employee { get; set; }

        public IEnumerable<EPAModel> MyEPAs { get; set; }

        public IEnumerable<EPAModel> MonthlyEPAs { get; set; }
    }
}