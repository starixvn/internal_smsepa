namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEPAEndEvaluationPeriodCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "EpaAuto_GetEPAEndEvaluationPeriod";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}