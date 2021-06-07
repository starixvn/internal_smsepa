namespace SMS.EPA.ServicesAPI.Criteria.JobLevel
{
    using Fanex.Data.Repository;

    public class GetLastModifiedDateForJobLevelCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "Sync_JobLevel_GetMaxModifiedDate";
    }
}