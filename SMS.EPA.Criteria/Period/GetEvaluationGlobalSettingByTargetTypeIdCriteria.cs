namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEvaluationGlobalSettingByTargetTypeIdCriteria : CriteriaBase
    {
        public int TargetTypeId { get; set; }

        public override bool IsValid()
        {
            return TargetTypeId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaPeriod_GetEvaluationGlobalSettingByTargetTypeId";
        }
    }
}