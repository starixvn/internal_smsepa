namespace SMS.EPA.Criteria.JobLevelStandard.Template
{
    using Fanex.Data.Repository;
    public class UpdateJobLevelStandardTemplateCriteria : CriteriaBase
    {
        public int TemplateId { get; set; }

        public string Name { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_Template_Update";

        public override bool IsValid() => TemplateId > 0 && ModifiedBy > 0 && !string.IsNullOrWhiteSpace(Name);
    }
}
