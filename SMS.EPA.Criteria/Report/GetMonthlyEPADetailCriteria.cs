namespace SMS.EPA.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetMonthlyEPADetailCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string PhaseIds { get; set; }

        public int? PeriodId { get; set; }

        public int? DepartmentId { get; set; }

        public bool? IsLocal { get; set; }

        public string SortColumnName { get; set; }

        public string SortDirection { get; set; }

        public int? PageIndex { get; set; }

        public int? ItemsPerPage { get; set; }

        public int? TotalItem { get; set; }

        public override string GetSettingKey()
        {
            return "Report_MonthlyEPADetail";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}