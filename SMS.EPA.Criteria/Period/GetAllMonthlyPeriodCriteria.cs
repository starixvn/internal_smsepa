namespace SMS.EPA.Criteria.Period
{
    using Fanex.Data.Repository;

    public class GetAllMonthlyPeriodCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaMonthlyPeriod_GetAll";
        }
    }
}