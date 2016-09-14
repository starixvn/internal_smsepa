namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetFlowTemplateByIdCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public override string GetSettingKey()
        {
            return "FlowTemplate_GetById";
        }

        public override bool IsValid()
        {
            return FlowTemplateId > 0;
        }
    }
}