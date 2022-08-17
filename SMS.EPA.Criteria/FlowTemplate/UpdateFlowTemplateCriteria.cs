namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateFlowTemplateCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public string Name { get; set; }

        public byte TargetTypeId { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "FlowTemplate_Update";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(Name) && FlowTemplateId > 0 && TargetTypeId > 0 && ModifiedBy > 0;
        }
    }
}