namespace SMS.EPA.ServicesAPI.Criteria.EmployeeDiscipline
{
    using Fanex.Data.Repository;

    public class GetFromDateForDisciplineCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Sync_EmployeeDiscipline_GetFromDate";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}