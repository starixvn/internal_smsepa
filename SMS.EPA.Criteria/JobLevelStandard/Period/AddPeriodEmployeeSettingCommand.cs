namespace SMS.EPA.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;

    public class AddPeriodEmployeeSettingCommand : CriteriaBase
    {
        public int PeriodId { get; set; }
        public string TeamIds { get; set; }
        public int? EmployeeId { get; set; }
        public int ModifiedBy { get; set; }

        public override bool IsValid() => PeriodId > 0;

        public override string GetSettingKey() => "JobLevelStandard_PeriodEmployeeSetting_Insert";
    }
}