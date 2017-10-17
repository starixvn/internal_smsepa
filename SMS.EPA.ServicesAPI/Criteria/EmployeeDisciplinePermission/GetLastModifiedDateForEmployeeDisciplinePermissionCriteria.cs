namespace SMS.EPA.ServicesAPI.Criteria.EmployeeDisciplinePermission
{
    using Fanex.Data.Repository;

    public class GetLastModifiedDateForEmployeeDisciplinePermissionCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Sync_EmployeeDisciplinePermission_GetMaxModifiedDate";

        public override bool IsValid() => true;
    }
}