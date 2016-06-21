namespace SMS.EPA.Criteria.Period
{
    using Fanex.Data.Repository;

    public class AddEmployeeToPeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public string Departments { get; set; }

        public int? EmployeeId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_AddEmployee";
        }
    }
}