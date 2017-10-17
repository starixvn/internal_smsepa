namespace SMS.EPA.ServicesAPI.Criteria.Team
{
    using Fanex.Data.Repository;

    public class GetLastModifiedDateForTeamCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Sync_Group_GetMaxModifiedDate";

        public override bool IsValid() => true;
    }
}