namespace SMS.EPA.Criteria.JobLevelStandard
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.JobLevelStandard.Period;

    public class CreateJobLevelStandardPeriodCommand : CriteriaBase
    {
        public string Name { get; set; }

        [SpParam(Ignored = true)]
        public IList<JobLevelStandardPeriodSetting> Settings { get; set; }

        public string SettingXml { get; set; }
        public bool IsActive { get; set; } = true;
        public int CreatedBy { get; set; }

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Name) && CreatedBy > 0;

        public override string GetSettingKey() => "JobLevelStandard_Period_Insert";
    }
}