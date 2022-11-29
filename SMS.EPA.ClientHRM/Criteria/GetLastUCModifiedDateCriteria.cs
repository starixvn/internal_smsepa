namespace SMS.EPA.ClientHrm.Criteria
{
    using Fanex.Data.Repository;

    public class GetLastUCModifiedDateCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Employee_GetLastUCModifiedDate";

        public override bool IsValid() => true;
    }
}
