namespace SMS.EPA.Criteria.JobLevelStandard
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetPeriodEmployeeSettingCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }
        [SpParam(Ignored = true)]
        public string PeriodName { get; set; }
        public string TeamIds { get; set; }
        public int? JobLevelId { get; set; }
        public bool? IsLocal { get; set; }
        public byte? StatusId { get; set; }
        public bool? IsExcluded { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_PeriodEmployeeSetting_Get";

        public override bool IsValid() => PeriodId > 0;
    }
}