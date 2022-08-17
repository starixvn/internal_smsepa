namespace SMS.EPA.Criteria
{
    using Core.Enumerations;
    using Fanex.Data.Repository;

    public class GetPeriodByActorIdCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public byte ProcessId { get; set; }

        public override bool IsValid() => ActorId > 0 && ProcessId >= (byte)ProcessType.Evaluator;

        public override string GetSettingKey() => "EPA_GetPeriodByActorId";
    }
}