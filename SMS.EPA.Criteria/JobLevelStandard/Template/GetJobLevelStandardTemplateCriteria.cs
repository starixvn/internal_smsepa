namespace SMS.EPA.Criteria.JobLevelStandard.Template
{
    using Fanex.Data.Repository;

    public class GetJobLevelStandardTemplateCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "JobLevelStandard_Template_Get";
    }
}