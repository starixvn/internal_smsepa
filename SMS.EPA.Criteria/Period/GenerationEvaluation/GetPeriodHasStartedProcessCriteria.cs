namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetPeriodHasStartedProcessCriteria : CriteriaBase
    {
        public int? PeriodId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_GetPeriodHasStartedProcess";
        }
    }
}