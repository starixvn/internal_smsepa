namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEvaluationGlobalSettingByTargetTypeIdCriteria : CriteriaBase
    {
        public byte TargetTypeId { get; set; }

        public override bool IsValid() => TargetTypeId > 0;

        public override string GetSettingKey() => "EpaPeriod_GetEvaluationGlobalSettingByTargetTypeId";
    }
}