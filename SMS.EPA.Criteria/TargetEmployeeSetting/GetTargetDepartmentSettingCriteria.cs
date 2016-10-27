namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetTargetDepartmentSettingCriteria : CriteriaBase
    {
        public int? DepartmentId { get; set; }

        public bool? IsGroup { get; set; }

        public int? TargetTypeId { get; set; }

        public bool ShowAll { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaTargetTemplateDepartment_Get";
        }
    }
}