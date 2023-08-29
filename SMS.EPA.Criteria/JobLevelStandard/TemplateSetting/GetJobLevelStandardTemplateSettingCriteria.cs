namespace SMS.EPA.Criteria.JobLevelStandard.TemplateSetting
{
    using Fanex.Data.Repository;
    public class GetJobLevelStandardTemplateSettingCriteria : CriteriaBase
    {
        public string ObjectIds { get; set; }
        public byte ObjectTypeId { get; set; }
        public bool ShowAll { get; set; }

        public override bool IsValid() => ObjectTypeId > 0;

        public override string GetSettingKey() => "JobLevelStandard_TemplateSetting_Get";
    }
}
