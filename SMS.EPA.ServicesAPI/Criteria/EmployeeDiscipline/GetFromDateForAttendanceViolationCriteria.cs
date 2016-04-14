namespace SMS.EPA.ServicesAPI.Criteria.EmployeeDiscipline
{
    using Fanex.Data.Repository;

    public class GetFromDateForAttendanceViolationCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Sync_EmployeeDiscipline_GetFromDate";
        }
    }
}