namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEvaluatorsByTargetTemplateCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int TargetTemplateId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_RatingByKPI_GetEvaluatorByTargetTemplate";
        }

        public override bool IsValid()
        {
            return PeriodId > 0 && TargetTemplateId > 0;
        }
    }
}