namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetTargetDepartmentSettingCriteria : CriteriaBase
    {
        public string DepartmentIds { get; set; }

        public int? TargetTypeId { get; set; }

        public bool ShowAll { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "EpaTargetTemplateDepartment_Get";
    }
}