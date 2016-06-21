namespace SMS.EPA.Criteria.TargetTemplate
{
    using Fanex.Data.Repository;

    public class DeleteTargetTemplateCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return TargetTemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "TargetTemplate_Delete";
        }
    }
}