namespace SMS.EPA.Criteria.EmployeeEPA.RatingByKPI
{
    using Fanex.Data.Repository;

    public class GetTargetTemplateByEvaluatorCriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public int PeriodId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_RatingByKPI_GetTargetTemplateByEvaluator";
        }

        public override bool IsValid()
        {
            return PeriodId > 0 && EvaluatorId > 0;
        }
    }
}