namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEPAEndEvaluationByPeriodIdCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int? GroupId { get; set; }

        public int? EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_GetEPAEndEvaluationByPeriodId";
        }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }
    }
}