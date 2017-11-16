namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetDepartmentSectionHasSettingCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public override bool IsValid() => FlowTemplateId > 0;

        public override string GetSettingKey() => "FlowTemplate_GetDepartmentHasSetting";
    }
}