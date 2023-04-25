namespace SMS.EPA.Criteria.JobLevelStandard.Template
{
    using Fanex.Data.Repository;
    public class UpdateStatusJobLevelStandardTemplateCriteria : CriteriaBase
    {
        public int TemplateId { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_Template_UpdateStatus";

        public override bool IsValid() => TemplateId > 0 && ModifiedBy > 0;
    }
}
