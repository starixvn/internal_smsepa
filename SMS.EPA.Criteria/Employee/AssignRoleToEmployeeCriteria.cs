namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class AssignRoleToEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string RoleIds { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0 && ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_AssignRoleToEmployee";
        }
    }
}