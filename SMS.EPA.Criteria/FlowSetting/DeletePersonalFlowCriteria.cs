namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class DeletePersonalFlowCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public int FlowTemplateId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0 && FlowTemplateId > 0 && ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "SettingEmployeeFlow_Delete";
        }
    }
}