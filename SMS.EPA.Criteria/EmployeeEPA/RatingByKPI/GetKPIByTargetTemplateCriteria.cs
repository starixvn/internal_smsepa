namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetKPIByTargetTemplateCriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public int PeriodId { get; set; }

        public int TargetTemplateId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_RatingByKPI_GetKPIByTargetTemplate";
        }

        public override bool IsValid()
        {
            return EvaluatorId > 0 && TargetTemplateId > 0;
        }
    }
}