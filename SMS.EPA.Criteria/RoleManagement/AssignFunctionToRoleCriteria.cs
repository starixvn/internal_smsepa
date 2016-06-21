namespace SMS.EPA.Criteria.RoleManagement
{
    using Fanex.Data.Repository;

    public class AssignFunctionToRoleCriteria : CriteriaBase
    {
        public int RoleId { get; set; }

        public string Functions { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return RoleId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaRole_AssignFunctionToRole";
        }
    }
}