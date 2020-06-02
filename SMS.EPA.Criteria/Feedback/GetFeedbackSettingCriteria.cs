namespace SMS.EPA.Criteria.Feedback
{
    using Fanex.Data.Repository;

    public class GetFeedbackSettingCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }
        public int? DepartmentId { get; set; }
        public int? EmployeeId { get; set; }

        public override bool IsValid() => PeriodId > 0;

        public override string GetSettingKey() => "Feedback_GetSetting";
    }
}