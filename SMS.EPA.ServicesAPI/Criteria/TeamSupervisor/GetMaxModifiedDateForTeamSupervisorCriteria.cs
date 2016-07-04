using Fanex.Data.Repository;

namespace SMS.EPA.ServicesAPI.Criteria.Permission
{
    public class GetMaxModifiedDateForTeamSupervisorCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Sync_TeamSupervisor_GetMaxModifiedDate";
        }
    }
}
