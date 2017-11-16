namespace SMS.EPA.Criteria
{
    using FlowSetting;

    public class CreateDepartmentFlowCriteria : CreateFlowCriteriaBase
    {
        public int DepartmentId { get; set; }

        public bool IsGroup { get; set; }

        public override bool IsValid() => DepartmentId > 0 && FlowTemplateId > 0 && ModifiedBy > 0;

        public override string GetSettingKey() => "SettingDepartmentFlow_Set";
    }
}