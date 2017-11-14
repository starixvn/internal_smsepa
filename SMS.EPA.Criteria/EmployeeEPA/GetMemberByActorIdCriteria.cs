namespace SMS.EPA.Criteria
{
    using Core.Enumerations;
    using Fanex.Data.Repository;

    public class GetMemberByActorIdCriteria : CriteriaBase
    {
        public int ProcessId { get; set; }

        public int ActorUserId { get; set; }

        public int? PeriodId { get; set; }

        public int? GroupId { get; set; }

        public bool? IsLocal { get; set; }

        public bool? IsMonthly { get; set; }

        public override bool IsValid() => ActorUserId > 0 && ProcessId >= (int)ProcessType.Evaluator;

        public override string GetSettingKey() => "EPA_GetMember";
    }
}