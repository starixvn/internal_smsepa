namespace SMS.EPA.Criteria.EmployeeEPA.RatingByKPI
{
    using Fanex.Data.Repository;

    public class GetEmployeeEPAToViewCriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public int PeriodId { get; set; }

        public int TargetTemplateId { get; set; }

        public int KPIId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_RatingByKPI_GetEmployeeToView";
        }

        public override bool IsValid()
        {
            return PeriodId > 0 && EvaluatorId > 0 && TargetTemplateId > 0 && KPIId > 0;
        }
    }
}