namespace SMS.EPA.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;

    public class EditPeriodEmployeeSettingCommand : CriteriaBase
    {
        public int PeriodId { get; set; }
        public int TemplateId { get; set; }
        public string SettingXml { get; set; }
        public int ModifiedBy { get; set; }

        public override bool IsValid() => PeriodId > 0;

        public override string GetSettingKey() => "JobLevelStandard_PeriodEmployeeSetting_Update";
    }
}