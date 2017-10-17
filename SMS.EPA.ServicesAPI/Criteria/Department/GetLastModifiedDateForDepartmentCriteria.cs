namespace SMS.EPA.ServicesAPI.Criteria.Department
{
    using Fanex.Data.Repository;

    public class GetLastModifiedDateForDepartmentCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "Sync_Department_GetMaxModifiedDate";
    }
}