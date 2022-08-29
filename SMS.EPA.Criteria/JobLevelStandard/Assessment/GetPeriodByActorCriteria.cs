namespace SMS.EPA.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;

    public class GetPeriodByActorCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_Assessment_GetPeriodByActorId";

        public override bool IsValid() => ActorId > 0;
    }
}