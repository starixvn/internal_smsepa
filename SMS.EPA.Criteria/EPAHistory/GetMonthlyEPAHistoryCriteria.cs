namespace SMS.EPA.Criteria.EPAHistory
{
    using Fanex.Data.Repository;

    public class GetMonthlyEPAHistoryCriteria : CriteriaBase
    {
        public int? PeriodId { get; set; }

        public int EvaluatorId { get; set; }

        public int? EmployeeId { get; set; }

        public override bool IsValid()
        {
            return EvaluatorId > 0;
        }

        public override string GetSettingKey()
        {
            return "Epa_GetMonthlyEPAHistory";
        }
    }
}
