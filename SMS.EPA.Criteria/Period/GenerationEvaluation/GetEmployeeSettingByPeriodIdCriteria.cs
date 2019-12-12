namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEmployeeSettingByPeriodIdCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int? ProcessId { get; set; }

        public string EmployeeIds { get; set; }

        public bool? IsEvaluated { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaAuto_GetEmployeeSettingToSendMail";
        }
    }
}