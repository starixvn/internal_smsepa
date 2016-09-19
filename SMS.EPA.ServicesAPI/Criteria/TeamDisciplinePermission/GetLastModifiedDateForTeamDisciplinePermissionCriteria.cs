using Fanex.Data.Repository;

namespace SMS.EPA.ServicesAPI.Criteria.TeamDisciplinePermission
{
    public class GetLastModifiedDateForTeamDisciplinePermissionCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Sync_TeamDisciplinePermission_GetMaxModifiedDate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
