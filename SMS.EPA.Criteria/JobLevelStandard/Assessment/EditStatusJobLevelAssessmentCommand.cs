namespace SMS.EPA.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Enumerations;

    public class EditStatusJobLevelAssessmentCommand : CriteriaBase
    {
        public string AssessmentIds { get; set; }
        public JobLevelAssessmentStatus StatusId { get; set; }
        public int ModifiedBy { get; set; }

        public override bool IsValid() => !string.IsNullOrWhiteSpace(AssessmentIds) && ModifiedBy > 0;

        public override string GetSettingKey() => "JobLevelStandard_Assessment_UpdateStatus";
    }
}