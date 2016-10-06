namespace SMS.EPA.Criteria.Period
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateEmployeeSettingOfPeriodCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        [SpParam(Ignored = true)]
        public string EmployeeIds { get; set; }

        public int? TargetTemplateId { get; set; }

        public string EmployeeAssignments { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_EditEmployeeSetting";
        }
    }
}