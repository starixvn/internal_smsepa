namespace SMS.EPA.ClientHRM.Criteria.UnitHead
{
    using Fanex.Data.Repository;

    public class GetUnitHeadScheduleCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Sync_GO_UnitHead_GetSchedule";

        public override bool IsValid() => true;
    }
}