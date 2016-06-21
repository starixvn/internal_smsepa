namespace SMS.EPA.Criteria.FlowManagement
{
    using Fanex.Data.Repository;

    public class DeleteFlowTemplateCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "FlowTemplate_Delete";
        }

        public override bool IsValid()
        {
            return FlowTemplateId > 0;
        }
    }
}