namespace SMS.EPA.Criteria.Department
{
    using Fanex.Data.Repository;

    public class GetDepartmentCriteria : CriteriaBase
    {
        public int? ParentId { get; set; }

        public bool? IsActive { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Department_Get";
        }
    }
}