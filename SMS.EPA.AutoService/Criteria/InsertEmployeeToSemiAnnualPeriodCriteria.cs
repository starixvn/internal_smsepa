namespace SMS.EPA.AutoService.Criteria
{
    using Fanex.Data.Repository;

    public class InsertEmployeeToSemiAnnualPeriodCriteria : CriteriaBase
    {
        public InsertEmployeeToSemiAnnualPeriodCriteria(int periodId = 0)
        {
            if (periodId > 0)
            {
                PeriodId = periodId;
            }
        }

        public int PeriodId { get; set; }

        public override string GetSettingKey()
        {
            return "EpaAuto_SemiAnnual_AddEmployeeToPeriod";
        }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }
    }
}