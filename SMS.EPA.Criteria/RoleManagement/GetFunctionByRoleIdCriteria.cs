namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetFunctionByRoleIdCriteria : CriteriaBase
    {
        public int RoleId { get; set; }

        public override bool IsValid()
        {
            return RoleId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaRole_GetFunctionByRoleId";
        }
    }
}