namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetDepartmentCriteria : CriteriaBase
    {
        public int? ParentId { get; set; }

        public bool? IsActive { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Department_Get";
    }
}