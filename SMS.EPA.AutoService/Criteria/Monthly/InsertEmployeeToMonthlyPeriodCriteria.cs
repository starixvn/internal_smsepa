namespace SMS.EPA.AutoService.Criteria
{
    using Fanex.Data.Repository;

    public class InsertEmployeeToMonthlyPeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public InsertEmployeeToMonthlyPeriodCriteria(int periodId = 0)
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
            return "EpaAuto_Monthly_AddEmployeeToPeriod";
        }
    }
}