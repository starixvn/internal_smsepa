namespace SMS.EPA.AutoService.Criteria
{
    using Fanex.Data.Repository;

    public class CheckLastestProcessOfPeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_CheckLastestEndEvaluationPeriod";
        }
    }
}