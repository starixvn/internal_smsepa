namespace SMS.EPA.Criteria.Period.GenerationEvaluation
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