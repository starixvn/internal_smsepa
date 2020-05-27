namespace SMS.EPA.Criteria.Feedback
{
    using Fanex.Data.Repository;

    public class GetFeedbackByIdCriteria : CriteriaBase
    {
        public long FeedbackId { get; set; }

        public override bool IsValid() => FeedbackId > 0;

        public override string GetSettingKey() => "Feedback_GetById";
    }
}