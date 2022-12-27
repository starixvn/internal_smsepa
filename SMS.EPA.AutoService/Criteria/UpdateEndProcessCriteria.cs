namespace SMS.EPA.AutoService.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateEndProcessCriteria : CriteriaBase
    {
        public string EPAProcess { get; set; }

        public override bool IsValid() => !string.IsNullOrWhiteSpace(EPAProcess);

        public override string GetSettingKey() => "EpaAuto_UpdateIsEndProcess";
    }
}