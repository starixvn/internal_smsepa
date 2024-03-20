namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class SearchEmployeeByRoleIdCriteria : CriteriaBase
    {
        public string Query { get; set; }
        public short RoleId { get; set; }

        public override bool IsValid() => RoleId > 0;

        public override string GetSettingKey() => "Employee_SearchByRoleId";
    }
}