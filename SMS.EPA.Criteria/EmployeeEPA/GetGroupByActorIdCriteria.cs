namespace SMS.EPA.Criteria
{
    using Core.Enumerations;
    using Fanex.Data.Repository;

    public class GetGroupByActorIdCriteria : CriteriaBase
    {
        public int ActorUserId { get; set; }

        public byte ProcessId { get; set; }

        public int? PeriodId { get; set; }

        public override bool IsValid() => ActorUserId > 0 && ProcessId >= (byte)ProcessType.Evaluator;

        public override string GetSettingKey() => "EPA_GetGroup";
    }
}