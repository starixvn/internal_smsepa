namespace SMS.EPA.Criteria.JobLevelStandard.TemplateSetting
{
    using Fanex.Data.Repository;
    public class DeleteJobLevelStandardTemplateSettingCriteria : CriteriaBase
    {
        public int ObjectId { get; set; }
        public int ObjectTypeId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid() => ObjectId > 0 && ObjectTypeId > 0 && ModifiedBy > 0;

        public override string GetSettingKey() => "JobLevelStandard_TemplateSetting_Delete";
    }
}
