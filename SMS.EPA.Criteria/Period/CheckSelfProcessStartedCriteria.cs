namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class CheckSelfProcessStartedCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public string EmployeeIds { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0 && !string.IsNullOrWhiteSpace(EmployeeIds);
        }

        public override string GetSettingKey()
        {
            return "EPAPeriod_CheckSelfProcessStarted";
        }
    }
}