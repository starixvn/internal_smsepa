namespace SMS.EPA.ServicesAPI.Criteria.TeamDisciplinePermission
{
    using Fanex.Data.Repository;

    public class GetLastModifiedDateForTeamDisciplinePermissionCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Sync_TeamDisciplinePermission_GetMaxModifiedDate";

        public override bool IsValid() => true;
    }
}