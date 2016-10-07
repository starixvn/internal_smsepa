namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class CreateFlowTemplateCriteria : CriteriaBase
    {
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
            return !string.IsNullOrEmpty(Name) && TargetTypeId > 0 && CreatedBy > 0;
        }
    }
}