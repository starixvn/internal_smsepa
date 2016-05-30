namespace SMS.EPA.Criteria.EmployeeEPA.RatingByKPI
{
    using Fanex.Data.Repository;

    public class GetPeriodByEvaluatorCriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_RatingByKPI_GetPeriodByEvaluator";
        }

        public override bool IsValid()
        {
            return EvaluatorId > 0;
        }
    }
}