namespace SMS.EPA.Criteria.EmployeeEPA.RatingByKPI
{
    using Fanex.Data.Repository;

    public class GetKPIToViewCriteria : CriteriaBase
    {
        public string EvaluatorIds { get; set; }

        public int TargetTemplateId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_RatingByKPI_GetKPIToView";
        }

        public override bool IsValid()
        {
            return TargetTemplateId > 0;
        }
    }
}