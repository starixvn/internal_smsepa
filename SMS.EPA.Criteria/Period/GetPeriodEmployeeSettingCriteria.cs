namespace SMS.EPA.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetPeriodEmployeeSettingCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        [SpParam(Ignored = true)]
        public string PeriodName { get; set; }

        public string TeamIds { get; set; }

        public bool? IsLocal { get; set; }

        public bool? IsExcludedEmployee { get; set; }

        public bool? IsGeneratedEPA { get; set; }

        public int? IsEvaluated { get; set; }

        public bool? HasMonthlyEPA { get; set; }

        public int? EPAStatus { get; set; }

        public override bool IsValid() => PeriodId > 0;

        public override string GetSettingKey() => "EpaPeriod_GetEmployeeSetting";
    }
}