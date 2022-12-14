namespace SMS.EPA.ClientHrm.Criteria
{
    using Fanex.Data.Repository;

    public class GetLastModifiedDateUserMappingCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Sync_UserMapping_GetLastModifiedDate";

        public override bool IsValid() => true;
    }
}
