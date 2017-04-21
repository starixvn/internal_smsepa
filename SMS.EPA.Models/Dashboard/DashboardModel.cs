namespace SMS.EPA.Model
{
    using Enitites;
    using System.Collections.Generic;

    public class DashboardModel
    {
        public Employee Employee { get; set; }

        public IEnumerable<EPAModel> MyEpaInfos { get; set; }

        public IEnumerable<EPAModel> MonthlyEpaInfos { get; set; }
    }
}