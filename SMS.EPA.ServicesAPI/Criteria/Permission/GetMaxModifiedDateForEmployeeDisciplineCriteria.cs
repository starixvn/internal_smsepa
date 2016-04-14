using Fanex.Data.Repository;

namespace SMS.EPA.ServicesAPI.Criteria.Permission
{
    public class GetMaxModifiedDateForEmployeeDisciplineCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Sync_EmployeeDisciplinePermission_GetMaxModifiedDate";
        }
    }
}
