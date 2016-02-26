namespace SMS.EPA.Criteria.Period
{
    using System;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Attribute;

    public class AutoCreateMonthlyPeriodCriteria : CriteriaBase
    {
        [Output]
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