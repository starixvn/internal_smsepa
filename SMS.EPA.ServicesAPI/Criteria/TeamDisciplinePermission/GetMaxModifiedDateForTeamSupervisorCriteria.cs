using Fanex.Data.Repository;

namespace SMS.EPA.ServicesAPI.Criteria.TeamDisciplinePermission
{
    public class GetMaxModifiedDateForTeamDisciplinePermissionCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Sync_TeamDisciplinePermission_GetMaxModifiedDate";
        }
    }
}
