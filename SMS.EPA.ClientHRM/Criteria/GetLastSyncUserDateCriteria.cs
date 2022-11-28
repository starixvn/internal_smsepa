namespace SMS.EPA.ClientHrm.Criteria
{
    using Fanex.Data.Repository;

    public class GetLastSyncUserDateCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Employee_GetLastSyncUserDate";

        public override bool IsValid() => true;
    }
}
