namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetPeriodToViewCriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_RatingByKPI_GetPeriodToView";
        }

        public override bool IsValid()
        {
            return EvaluatorId > 0;
        }
    }
}