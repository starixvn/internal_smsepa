namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateStatusFlowTemplateCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public bool IsActive { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "FlowTemplate_UpdateStatus";
        }

        public override bool IsValid()
        {
            return FlowTemplateId > 0;
        }
    }
}