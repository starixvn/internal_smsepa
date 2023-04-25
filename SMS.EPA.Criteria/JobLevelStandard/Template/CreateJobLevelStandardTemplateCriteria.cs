

namespace SMS.EPA.Criteria.JobLevelStandard.Template
{
    using Fanex.Data.Repository;
    public class CreateJobLevelStandardTemplateCriteria : CriteriaBase
    {
        public int TemplateId { get; set; }

        public string Name { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_Template_Insert";

        public override bool IsValid() => CreatedBy > 0 && !string.IsNullOrWhiteSpace(Name);
    }
}
