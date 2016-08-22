namespace SMS.EPA.AutoGenerateEvaluation.Criteria
{
    using Fanex.Data.Repository;

    public class InsertEmployeeToMonthlyPeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

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