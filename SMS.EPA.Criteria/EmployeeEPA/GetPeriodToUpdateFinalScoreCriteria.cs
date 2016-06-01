namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetPeriodToUpdateFinalScoreCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "EPA_GetPeriodToUpdateFinalScore";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}