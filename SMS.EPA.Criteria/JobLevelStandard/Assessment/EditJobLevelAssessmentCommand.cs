namespace SMS.EPA.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Enumerations;

    public class EditJobLevelAssessmentCommand : CriteriaBase
    {
        public int AssessmentId { get; set; }
        public int PeriodId { get; set; }
        public int EmployeeId { get; set; }
        public decimal? TotalWeight { get; set; }
        public int? CurrentJobLevelId { get; set; }
        public int? NewJobLevelId { get; set; }
        public JobLevelAssessmentStatus? StatusId { get; set; }
        public string Remark { get; set; }
        public string Comment { get; set; }
        public string AssessmentKPIXml { get; set; }
        public int ModifiedBy { get; set; }

        public override bool IsValid() => ModifiedBy > 0;

        public override string GetSettingKey() => "JobLevelStandard_Assessment_Update";
    }
}