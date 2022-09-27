namespace SMS.EPA.AutoService.Criteria.JobLevelStandard
{
    using Fanex.Data.Repository;

    public class GetJobLevelStandardPeriodEmployeeSettingToSendMailCriteria : CriteriaBase
    {
        public int? PeriodId { get; set; }
        public override bool IsValid() => true;

        public override string GetSettingKey() => "Auto_JobLevelStandard_GetEmployeeSettingToSendMail";
    }
}