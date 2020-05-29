namespace SMS.EPA.Criteria.Report
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetFeedbackReportCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }
        public int? DepartmentId { get; set; }
        public int? EmployeeId { get; set; }

        [SpParam(Ignored = true)]
        public string Period { get; set; }

        public override bool IsValid() => PeriodId > 0;

        public override string GetSettingKey() => "Report_Feedback";
    }
}