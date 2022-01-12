namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEmployeeLoginByUserIdCriteria : CriteriaBase
    {
        public int UserId { get; set; }

        public override bool IsValid() => UserId > 0;

        public override string GetSettingKey() => "Employee_GetByUserId";
    }
}