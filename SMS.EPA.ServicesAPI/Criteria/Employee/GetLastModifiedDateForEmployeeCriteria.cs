namespace SMS.EPA.ServicesAPI.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetLastModifiedDateForEmployeeCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Sync_Employee_GetMaxModifiedDate";

        public override bool IsValid() => true;
    }
}