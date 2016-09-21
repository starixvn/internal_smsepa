namespace SMS.EPA.Criteria.EPAHistory
{
    using Fanex.Data.Repository;

    public class GetEPAHistoryCriteria : CriteriaBase
    {
        public int? PeriodId { get; set; }

        public int EvaluatorId { get; set; }

        public int? DepartmentId { get; set; }

        public bool? IsLocal { get; set; }

        public int? EmployeeId { get; set; }

        public override bool IsValid()
        {
            return EvaluatorId > 0;
        }

        public override string GetSettingKey()
        {
            return "Epa_GetEPAHistory";
        }
    }
}