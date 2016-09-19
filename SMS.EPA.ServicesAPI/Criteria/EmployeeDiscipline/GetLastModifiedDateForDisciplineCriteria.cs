namespace SMS.EPA.ServicesAPI.Criteria.EmployeeDiscipline
{
    using Fanex.Data.Repository;

    public class GetLastModifiedDateForDisciplineCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Sync_EmployeeDiscipline_GetMaxModifiedDate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}