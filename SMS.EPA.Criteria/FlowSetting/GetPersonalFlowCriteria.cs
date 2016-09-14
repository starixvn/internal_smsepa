namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetPersonalFlowCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public override bool IsValid()
        {
            return FlowTemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "SettingEmployeeFlow_Get";
        }
    }
}