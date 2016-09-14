namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Attribute;

    public class CreateFlowTemplateCriteria : CriteriaBase
    {
        [Output]
        public int FlowTemplateId { get; set; }

        public string Name { get; set; }

        public int TargetTypeId { get; set; }

        public string Description { get; set; }

        public bool IsActive { get; set; }

        public int CreatedBy { get; set; }

        public override string GetSettingKey()
        {
            return "FlowTemplate_Insert";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Name) && TargetTypeId > 0;
        }
    }
}