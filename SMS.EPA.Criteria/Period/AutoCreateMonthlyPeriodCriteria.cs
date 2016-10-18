namespace SMS.EPA.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class AutoCreateMonthlyPeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public string Name { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_Monthly_InsertPeriod";
        }
    }
}