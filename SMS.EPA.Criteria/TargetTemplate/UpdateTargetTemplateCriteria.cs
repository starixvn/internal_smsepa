namespace SMS.EPA.Criteria.TargetTemplate
{
    using Fanex.Data.Repository;

    public class UpdateTargetTemplateCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public string KPIs { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return TargetTemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "TargetTemplate_Update";
        }
    }
}