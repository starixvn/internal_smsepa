namespace SMS.EPA.ServicesAPI.Criteria.Department
{
    using Fanex.Data.Repository;

    public class GetLastModifiedDateForDepartmentCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Sync_Department_GetMaxModifiedDate";
        }
    }
}