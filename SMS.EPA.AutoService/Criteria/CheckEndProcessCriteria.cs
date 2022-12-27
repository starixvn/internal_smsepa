namespace SMS.EPA.AutoService.Criteria
{
    using Fanex.Data.Repository;

    public class CheckEndProcessCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "EpaAuto_CheckEndProcess";
    }
}