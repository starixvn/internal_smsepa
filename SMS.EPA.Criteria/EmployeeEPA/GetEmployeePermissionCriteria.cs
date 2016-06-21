namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEmployeePermissionCriteria : CriteriaBase
    {
        public int EPAId { get; set; }

        public int ActorUserId { get; set; }

        public int? ProcessId { get; set; }

        public override bool IsValid()
        {
            return EPAId > 0 && ActorUserId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_CheckPermission";
        }
    }
}