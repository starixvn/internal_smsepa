namespace SMS.EPA.Criteria.JobLevelStandard
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.JobLevelStandard;

    public class EditPeriodJobLevelSettingCommand : CriteriaBase
    {
        public int PeriodId { get; set; }
        public int SectionId { get; set; }

        [SpParam(Ignored = true)]
        public IList<JobLevelSetting> Settings { get; set; }

        public string SettingXml { get; set; }
        public int ModifiedBy { get; set; }

        public override bool IsValid() => PeriodId > 0 && SectionId > 0 && ModifiedBy > 0;

        public override string GetSettingKey() => "JobLevelStandard_PeriodJobLevelSetting_Update";
    }
}