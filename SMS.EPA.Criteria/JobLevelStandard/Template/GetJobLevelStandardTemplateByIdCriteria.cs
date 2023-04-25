namespace SMS.EPA.Criteria.JobLevelStandard.Template
{
    using Fanex.Data.Repository;
    public class GetJobLevelStandardTemplateByIdCriteria : CriteriaBase
    {
        public int TemplateId { get; set; }

        public override bool IsValid()
        {
            return TemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "JobLevelStandard_Template_GetById";
        }
    }
}
