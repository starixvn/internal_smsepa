namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetPersonalFlowByIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int FlowTemplateId { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0 && FlowTemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "SettingEmployeeFlow_GetById";
        }
    }
}