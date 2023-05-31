namespace SMS.EPA.Criteria.JobLevelStandard.TemplateSetting
{
    using Fanex.Data.Repository;

    public class AssignJobLevelStandardTemplateSettingCriteria : CriteriaBase
    {
        public int TemplateId { get; set; }

        public string ObjectIds { get; set; }

        public int ObjectTypeId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid() => TemplateId > 0 && ModifiedBy > 0;

        public override string GetSettingKey() => "JobLevelStandard_TemplateSetting_Assign";
    }

}
