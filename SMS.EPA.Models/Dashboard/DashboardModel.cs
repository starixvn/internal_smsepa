namespace SMS.EPA.Model
{
    using System.Collections.Generic;
    using Enitites;

    public class DashboardModel
    {
        public Employee Employee { get; set; }

        public IEnumerable<EPAModel> MyEpaInfos { get; set; }

        public IEnumerable<EPAModel> MonthlyEpaInfos { get; set; }
    }
}