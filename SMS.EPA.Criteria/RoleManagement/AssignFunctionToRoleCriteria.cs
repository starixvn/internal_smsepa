namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class AssignFunctionToRoleCriteria : CriteriaBase
    {
        public int RoleId { get; set; }

        public string Functions { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return RoleId > 0 && ModifiedBy > 0 && !string.IsNullOrEmpty(Functions);
        }

        public override string GetSettingKey()
        {
            return "EpaRole_AssignFunctionToRole";
        }
    }
}