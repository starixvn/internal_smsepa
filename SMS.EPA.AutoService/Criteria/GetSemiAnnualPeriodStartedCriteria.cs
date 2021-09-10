namespace SMS.EPA.AutoService.Criteria
{
    using Fanex.Data.Repository;

    public class GetSemiAnnualPeriodStartedCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "EpaAuto_SemiAnnual_GetPeriodStartedToday";
    }
}