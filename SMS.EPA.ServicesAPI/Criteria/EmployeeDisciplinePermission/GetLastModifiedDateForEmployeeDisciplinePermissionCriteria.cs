using Fanex.Data.Repository;

namespace SMS.EPA.ServicesAPI.Criteria.EmployeeDisciplinePermission
{
    public class GetLastModifiedDateForEmployeeDisciplinePermissionCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Sync_EmployeeDisciplinePermission_GetMaxModifiedDate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
