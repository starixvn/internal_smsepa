namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetOrgChartCriteria : CriteriaBase
    {
        public override bool IsValid() => true;

        public override string GetSettingKey() => "Department_GetOrgChart";
    }
}