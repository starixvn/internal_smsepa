namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using SMS.EPA.Model.Enitites;

    public class DashboardModel
    {
        public Employee Employee { get; set; }

        public IEnumerable<EPAModel> MyEPAs { get; set; }

        public IEnumerable<EPAModel> MonthlyEPAs { get; set; }
    }
}