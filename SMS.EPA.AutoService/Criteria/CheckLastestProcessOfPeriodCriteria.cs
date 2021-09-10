namespace SMS.EPA.AutoService.Criteria
{
    using Fanex.Data.Repository;

    public class CheckLastestProcessOfPeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override bool IsValid() => PeriodId > 0;

        public override string GetSettingKey() => "EpaAuto_CheckLastestEndEvaluationPeriod";
    }
}