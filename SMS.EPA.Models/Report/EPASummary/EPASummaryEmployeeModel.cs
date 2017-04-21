namespace SMS.EPA.Model
{
    using Enitites;
    using System;
    using System.Collections.Generic;

    public class EPASummaryEmployeeModel
    {
        public EPASummaryEmployeeModel()
        {
            Periods = new List<EPASummaryPeriodModel>();
        }

        public int EmployeeId { get; set; }

        public string Code { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public Group Group { get; set; }

        public DateTime JoinedDate { get; set; }

        public JobTitle JobTitle { get; set; }

        public string Category { get; set; }

        public IList<EPASummaryPeriodModel> Periods { get; set; }
    }
}