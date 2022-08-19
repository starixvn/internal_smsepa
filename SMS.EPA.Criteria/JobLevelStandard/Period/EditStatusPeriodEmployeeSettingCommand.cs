namespace SMS.EPA.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;

    public class EditStatusPeriodEmployeeSettingCommand : CriteriaBase
    {
        public int PeriodId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsExcluded { get; set; }
        public int ModifiedBy { get; set; }

        public override bool IsValid() => PeriodId > 0 && EmployeeId > 0 && ModifiedBy > 0;

        public override string GetSettingKey() => "JobLevelStandard_PeriodEmployeeSetting_UpdateStatus";
    }
}