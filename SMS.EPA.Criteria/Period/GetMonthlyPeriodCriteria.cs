namespace SMS.EPA.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetMonthlyPeriodCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaMonthlyPeriod_Get";
        }
    }
}