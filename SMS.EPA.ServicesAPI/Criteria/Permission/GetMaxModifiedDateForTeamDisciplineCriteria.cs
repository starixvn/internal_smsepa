using Fanex.Data.Repository;

namespace SMS.EPA.ServicesAPI.Criteria.Permission
{
    public class GetMaxModifiedDateForTeamDisciplineCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Sync_TeamDisciplinePermission_GetMaxModifiedDate";
        }
    }
}
