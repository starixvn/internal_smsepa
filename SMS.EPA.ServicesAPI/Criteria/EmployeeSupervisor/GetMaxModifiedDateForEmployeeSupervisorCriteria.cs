using Fanex.Data.Repository;

namespace SMS.EPA.ServicesAPI.Criteria.Permission
{
    public class GetMaxModifiedDateForEmployeeSupervisorCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Sync_EmployeeSupervisor_GetMaxModifiedDate";
        }
    }
}
