namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetKPIGroupCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "KPIGroup_Get";
        }
    }
}