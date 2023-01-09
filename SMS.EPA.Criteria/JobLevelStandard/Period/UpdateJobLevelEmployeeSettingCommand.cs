using Fanex.Data.Repository;

namespace SMS.EPA.Criteria.JobLevelStandard.Period
{
    public class UpdateJobLevelEmployeeSettingCommand : CriteriaBase
    {
        public int AssessmentId { get; set; }
        public int EmployeeId { get; set; }
        public int ModifiedBy { get; set; }
        public override bool IsValid() => AssessmentId > 0;

        public override string GetSettingKey() => "JobLevelStandard_Period_UpdateJobLevel";
    }
}
