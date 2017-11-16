namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class AssignTargetDepartmentSettingCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public string Departments { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid() => TargetTemplateId > 0 && ModifiedBy > 0;

        public override string GetSettingKey() => "EpaTargetTemplateDepartment_Assign";
    }
}