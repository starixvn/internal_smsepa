namespace SMS.EPA.Criteria.Feedback
{
    using System;
    using Fanex.Data.Repository;

    public class UpdateFeedbackSettingCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }
        public string EmployeeIds { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public override bool IsValid() => PeriodId > 0 && !string.IsNullOrWhiteSpace(EmployeeIds);

        public override string GetSettingKey() => "Feedback_UpdateSetting";
    }
}