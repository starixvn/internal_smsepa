namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEPAByReviewerCriteria : CriteriaBase
    {
        public int ReviewerId { get; set; }

        public string EmployeeIds { get; set; }

        public int? GroupId { get; set; }

        public bool? IsLocal { get; set; }

        public bool? StatusProcess { get; set; }

        public bool IsMonthly { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_GetByReviewer";
        }

        public override bool IsValid()
        {
            return ReviewerId > 0;
        }
    }
}