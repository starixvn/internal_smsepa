namespace SMS.EPA.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class UpdateUserIdCommand : CriteriaBase
    {
        public int EmployeeId { get; set; }
        public int UserId { get; set; }

        public override string GetSettingKey() => "Employee_UpdateUserId";

        public override bool IsValid() => EmployeeId > 0 && UserId > 0;
    }
}