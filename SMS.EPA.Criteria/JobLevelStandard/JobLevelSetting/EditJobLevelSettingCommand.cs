namespace SMS.EPA.Criteria.JobLevelStandard
{
    using System.Collections.Generic;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.JobLevelStandard;

    public class EditJobLevelSettingCommand : CriteriaBase
    {
        public int SectionId { get; set; }

        [SpParam(Ignored = true)]
        public IList<JobLevelSetting> Settings { get; set; }

        public string SettingXml { get; set; }
        public int ModifiedBy { get; set; }

        public override bool IsValid() => SectionId > 0 && ModifiedBy > 0;

        public override string GetSettingKey() => "JobLevelStandard_JobLevelSetting_Update";
    }
}