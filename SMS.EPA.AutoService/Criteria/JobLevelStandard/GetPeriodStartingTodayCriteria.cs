namespace SMS.EPA.AutoService.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;

    public class GetPeriodStartingTodayCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "Auto_JobLevelStandard_GetPeriodStartingToday";
    }
}