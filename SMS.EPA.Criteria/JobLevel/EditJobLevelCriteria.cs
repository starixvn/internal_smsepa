namespace SMS.EPA.Criteria.JobLevel
{
    using Fanex.Data.Repository;

    public class EditJobLevelCriteria : CriteriaBase
    {
        public int JobLevelId { get; set; }
        public string Name { get; set; }

        public override string GetSettingKey() => "JobLevel_Update";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Name);
    }
}