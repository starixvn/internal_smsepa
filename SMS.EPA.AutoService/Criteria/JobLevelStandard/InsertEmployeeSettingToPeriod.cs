namespace SMS.EPA.AutoService.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;

    public class InsertEmployeeSettingToPeriod : CriteriaBase
    {
        public int PeriodId { get; set; }

        public override bool IsValid() => PeriodId > 0;

        public override string GetSettingKey() => "Auto_JobLevelStandard_AddEmployeeSetting";
    }
}