namespace SMS.EPA.Criteria.JobLevel
{
    using Fanex.Data.Repository;

    public class CreateJobLevelCriteria : CriteriaBase
    {
        public string Name { get; set; }

        public override string GetSettingKey() => "JobLevel_Insert";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Name);
    }
}