namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEmployeeEPAByKPICriteria : CriteriaBase
    {
        public int EvaluatorId { get; set; }

        public int PeriodId { get; set; }

        public int TargetTemplateId { get; set; }

        public int KPIId { get; set; }

        public int? GroupId { get; set; }

        public string EmployeeIds { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_RatingByKPI_Get";
        }

        public override bool IsValid()
        {
            return EvaluatorId > 0 && PeriodId > 0 && TargetTemplateId > 0 && KPIId > 0;
        }
    }
}