namespace SMS.EPA.Criteria
{
    using System;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetDisciplineSummaryForOtherSystemCriteria : CriteriaBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? DepartmentId { get; set; }

        public bool? IsLocal { get; set; }

        public int? EmployeeId { get; set; }

        public int? ReasonId { get; set; }

        [SpParam(Ignored = true)]
        public string EmployeeName { get; set; }

        [SpParam(Ignored = true)]
        public string DepartmentName { get; set; }

        public override bool IsValid() => EndDate >= StartDate;

        public override string GetSettingKey()
        {
            return "DisciplineSummary_GetForOtherSystem";
        }
    }
}