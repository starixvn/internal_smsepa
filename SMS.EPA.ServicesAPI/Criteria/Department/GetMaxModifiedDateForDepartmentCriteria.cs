namespace SMS.EPA.ServicesAPI.Criteria.Department
{
    using Fanex.Data.Repository;

    public class GetMaxModifiedDateForDepartmentCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Sync_Department_GetMaxModifiedDate";
        }
    }
}