namespace SMS.EPA.AutoService.Criteria
{
    using Fanex.Data.Repository;

    public class InsertEmployeeToProbationPeriodCriteria : CriteriaBase
    {
        public int? PeriodId { get; set; }

        public string EmployeeIds { get; set; }

        public override bool IsValid()
        {
            return PeriodId != null && PeriodId > 0 && !string.IsNullOrWhiteSpace(EmployeeIds);
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_Probation_AddEmployeeToPeriod";
        }
    }
}