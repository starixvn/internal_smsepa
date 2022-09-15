namespace SMS.EPA.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetEPADetailCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int? EmployeeId { get; set; }
        public string PhaseIds { get; set; }
        public int? PeriodId { get; set; }
        public string TeamIds { get; set; }
        public bool? IsLocal { get; set; }
        public string SortColumnName { get; set; }
        public string SortDirection { get; set; }

        public override string GetSettingKey() => "Report_EPADetail";

        public override bool IsValid() => ActorId > 0;
    }
}