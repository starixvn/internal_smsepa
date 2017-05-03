namespace SMS.EPA.Criteria
{
    using FlowSetting;

    public class CreatePersonalFlowCriteria : CreateFlowCriteriaBase
    {
        public string EmployeeIds { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EmployeeIds) && FlowTemplateId > 0 && ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "SettingEmployeeFlow_Set";
        }
    }
}