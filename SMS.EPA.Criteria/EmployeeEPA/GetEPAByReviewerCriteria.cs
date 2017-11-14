namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEPAByReviewerCriteria : CriteriaBase
    {
        public int ReviewerId { get; set; }

        public string EmployeeIds { get; set; }

        public int? PeriodId { get; set; }

        public int? GroupId { get; set; }

        public bool? IsLocal { get; set; }

        public bool? StatusProcess { get; set; }

        public bool IsMonthly { get; set; }

        public override string GetSettingKey() => "EPA_GetByReviewer";

        public override bool IsValid() => ReviewerId > 0;
    }
}