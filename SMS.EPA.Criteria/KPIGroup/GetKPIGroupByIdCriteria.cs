namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetKPIGroupByIdCriteria : CriteriaBase
    {
        public int KPIGroupId { get; set; }

        public override bool IsValid()
        {
            return KPIGroupId > 0;
        }

        public override string GetSettingKey()
        {
            return "KPIGroup_GetById";
        }
    }
}