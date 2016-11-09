namespace SMS.EPA.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetEPADetailCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public int? EmployeeId { get; set; }

        public string PhaseIds { get; set; }

        public int? PeriodId { get; set; }

        public int? DepartmentId { get; set; }

        public bool? IsLocal { get; set; }

        public string SortColumnName { get; set; }

        public string SortDirection { get; set; }

        public override string GetSettingKey()
        {
            return "Report_EPADetail";
        }

        public override bool IsValid()
        {
            return ActorId > 0;
        }
    }
}