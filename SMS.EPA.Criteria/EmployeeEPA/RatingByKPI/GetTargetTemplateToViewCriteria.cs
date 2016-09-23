namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetTargetTemplateToViewCriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public int PeriodId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_RatingByKPI_GetTargetTemplateToView";
        }

        public override bool IsValid()
        {
            return PeriodId > 0 && EvaluatorId > 0;
        }
    }
}