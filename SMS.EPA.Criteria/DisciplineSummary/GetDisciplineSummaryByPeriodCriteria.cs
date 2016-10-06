namespace SMS.EPA.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetDisciplineSummaryByPeriodCriteria : CriteriaBase
    {
        public int? PeriodId { get; set; }

        public int? DepartmentId { get; set; }

        public bool? IsLocal { get; set; }

        public int? EmployeeId { get; set; }

        public int? DisciplineTypeId { get; set; }

        [SpParam(Ignored = true)]
        public string EmployeeName { get; set; }

        [SpParam(Ignored = true)]
        public string DepartmentName { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EmployeeDisciplineSummary_GetByPeriod";
        }
    }
}