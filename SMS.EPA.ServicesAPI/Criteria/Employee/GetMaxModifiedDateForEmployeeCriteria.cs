using Fanex.Data.Repository;

namespace SMS.EPA.ServicesAPI.Criteria.Employee
{
    public class GetMaxModifiedDateForEmployeeCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Sync_Employee_GetMaxModifiedDate";
        }
    }
}
