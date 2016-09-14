namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetDepartmentFlowCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public override bool IsValid()
        {
            return FlowTemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "SettingDepartmentFlow_Get";
        }
    }
}