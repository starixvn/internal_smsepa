namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class DeleteTargetTemplateCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return TargetTemplateId > 0 && ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "TargetTemplate_Delete";
        }
    }
}