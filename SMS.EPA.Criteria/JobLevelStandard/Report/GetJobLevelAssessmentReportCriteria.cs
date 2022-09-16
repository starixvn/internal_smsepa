namespace SMS.EPA.Criteria.JobLevelStandard.Report
{
    using Fanex.Data.Repository;

    public class GetJobLevelAssessmentReportCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int PeriodId { get; set; }
        public string SectionIds { get; set; }
        public bool? IsLocal { get; set; }
        public byte? StatusId { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_AssessmentReport_Get";

        public override bool IsValid() => PeriodId > 0 && ActorId > 0;
    }
}