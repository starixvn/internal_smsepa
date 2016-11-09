namespace SMS.EPA.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetEPASummaryCriteria : CriteriaBase
    {
        public string PeriodIds { get; set; }

        public int ActorId { get; set; }

        public int? EmployeeId { get; set; }

        public int? DepartmentId { get; set; }

        public bool? IsLocal { get; set; }

        public string SortColumnName { get; set; }

        public string SortDirection { get; set; }

        public override string GetSettingKey()
        {
            return "Report_EPASummary";
        }

        public override bool IsValid()
        {
            return ActorId > 0;
        }
    }
}