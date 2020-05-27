namespace SMS.EPA.Criteria.Feedback
{
    using Fanex.Data.Repository;

    public class UpdateFeedbackCriteria : CriteriaBase
    {
        public long FeedbackId { get; set; }
        public string FinalScore { get; set; }
        public string Comment { get; set; }
        public string DetailXml { get; set; }

        public override bool IsValid() => FeedbackId > 0;

        public override string GetSettingKey() => "Feedback_Update";
    }
}