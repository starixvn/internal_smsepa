namespace SMS.EPA.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;

    public class GetJobLevelAssessmentByActorIdCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int PeriodId { get; set; }
        public string SectionIds { get; set; }
        public byte? StatusId { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_Assessment_GetByActorId";

        public override bool IsValid() => PeriodId > 0 && ActorId > 0;
    }
}