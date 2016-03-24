namespace SMS.EPA.Criteria.EmployeeEPA.RatingByKPI
{
    using Fanex.Data.Repository;

    public class GetEvaluatorsByTargetTemplateCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_RatingByKPI_GetEvaluatorByTargetTemplate";
        }

        public override bool IsValid()
        {
            return TargetTemplateId > 0;
        }
    }
}