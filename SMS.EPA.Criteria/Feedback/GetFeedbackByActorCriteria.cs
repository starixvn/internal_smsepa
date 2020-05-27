namespace SMS.EPA.Criteria.Feedback
{
    using Fanex.Data.Repository;

    public class GetFeedbackByActorCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public int PeriodId { get; set; }

        public override bool IsValid() => ActorId > 0;

        public override string GetSettingKey() => "Feedback_GetByActor";
    }
}