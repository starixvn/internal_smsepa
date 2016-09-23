namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEPAInfoByReviewerCriteria : CriteriaBase
    {
        public int EPAId { get; set; }

        public int ReviewerId { get; set; }

        public override bool IsValid()
        {
            return EPAId > 0 && ReviewerId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_GetInfoByReviewer";
        }
    }
}