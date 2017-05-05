namespace SMS.EPA.AutoService.Criteria
{
    using Fanex.Data.Repository;

    public class InsertEmployeeToSemiAnnualPeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public InsertEmployeeToSemiAnnualPeriodCriteria(int periodId = 0)
        {
            if (periodId > 0)
            {
                PeriodId = periodId;
            }
        }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_SemiAnnual_AddEmployeeToPeriod";
        }
    }
}