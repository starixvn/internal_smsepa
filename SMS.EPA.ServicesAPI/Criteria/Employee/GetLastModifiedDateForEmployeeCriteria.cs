using Fanex.Data.Repository;

namespace SMS.EPA.ServicesAPI.Criteria.Employee
{
    public class GetLastModifiedDateForEmployeeCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Sync_Employee_GetMaxModifiedDate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
