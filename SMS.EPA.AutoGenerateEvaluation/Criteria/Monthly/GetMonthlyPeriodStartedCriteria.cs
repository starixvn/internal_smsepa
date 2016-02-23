namespace SMS.EPA.AutoGenerateEvaluation.Criteria.Monthly
{
    using Fanex.Data.Repository;

    public class GetMonthlyPeriodStartedCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_Monthly_GetPeriodStartedToday";
        }
    }
}