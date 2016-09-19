using Fanex.Data.Repository;

namespace SMS.EPA.ServicesAPI.Criteria.Team
{
    public class GetLastModifiedDateForTeamCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Sync_Group_GetMaxModifiedDate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
