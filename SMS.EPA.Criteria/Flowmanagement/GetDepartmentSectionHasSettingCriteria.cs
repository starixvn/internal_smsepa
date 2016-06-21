namespace SMS.EPA.Criteria.FlowManagement
{
    using Fanex.Data.Repository;

    public class GetDepartmentSectionHasSettingCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public override bool IsValid()
        {
            return FlowTemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "FlowTemplate_GetDepartmentHasSetting";
        }
    }
}