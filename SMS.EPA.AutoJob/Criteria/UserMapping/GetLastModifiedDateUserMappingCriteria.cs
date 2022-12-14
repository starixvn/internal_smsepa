namespace SMS.EPA.AutoJob.Criteria
{
    using Fanex.Data.Repository;

    internal class GetLastModifiedDateUserMappingCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Sync_UserMapping_GetLastModifiedDate";

        public override bool IsValid() => true;
    }
}
