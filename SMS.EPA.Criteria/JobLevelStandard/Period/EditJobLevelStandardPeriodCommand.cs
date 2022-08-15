namespace SMS.EPA.Criteria.JobLevelStandard
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.JobLevelStandard.Period;

    public class EditJobLevelStandardPeriodCommand : CriteriaBase
    {
        public int PeriodId { get; set; }
        public string Name { get; set; }

        [SpParam(Ignored = true)]
        public IList<JobLevelStandardPeriodSetting> Settings { get; set; }

        public string SettingXml { get; set; }
        public int ModifiedBy { get; set; }

        public override bool IsValid() => PeriodId > 0 && ModifiedBy > 0;

        public override string GetSettingKey() => "JobLevelStandard_Period_Update";
    }
}