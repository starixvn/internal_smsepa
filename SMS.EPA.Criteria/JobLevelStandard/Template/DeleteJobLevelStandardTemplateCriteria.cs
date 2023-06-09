namespace SMS.EPA.Criteria.JobLevelStandard.Template
{
    using Fanex.Data.Repository;

    public class DeleteJobLevelStandardTemplateCriteria : CriteriaBase
    {
        public int TemplateId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid() => TemplateId > 0 && ModifiedBy > 0;

        public override string GetSettingKey() => "JobLevelStandard_Template_Delete";
    }
}