namespace SMS.EPA.AutoService.Criteria
{
    using Fanex.Data.Repository;

    public class GetEmployeePromotionEvaluationCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_Promotion_GetEmployees";
        }
    }
}