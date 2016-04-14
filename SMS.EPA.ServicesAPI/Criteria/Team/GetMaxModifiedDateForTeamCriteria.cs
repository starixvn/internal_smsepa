using Fanex.Data.Repository;

namespace SMS.EPA.ServicesAPI.Criteria.Team
{
    public class GetMaxModifiedDateForTeamCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Sync_Group_GetMaxModifiedDate";
        }
    }
}
