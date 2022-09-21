namespace SMS.EPA.Criteria.EPAHistory
{
    using Fanex.Data.Repository;

    public class GetEPAHistoryCriteria : CriteriaBase
    {
        public int? PeriodId { get; set; }
        public int EvaluatorId { get; set; }
        public string TeamIds { get; set; }

        public bool? IsLocal { get; set; }
        public int? EmployeeId { get; set; }

        public override bool IsValid() => EvaluatorId > 0;

        public override string GetSettingKey() => "Epa_GetEPAHistory";
    }
}