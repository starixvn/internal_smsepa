namespace SMS.EPA.AutoService.Criteria
{
    using Fanex.Data.Repository;

    public class GetEmployeeProbationEvaluationCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_Probation_GetEmployees";
        }
    }
}