namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class DeleteKPIGroupCriteria : CriteriaBase
    {
        public int KPIGroupId { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "KPIGroup_Delete";
        }

        public override bool IsValid()
        {
            return KPIGroupId > 0 && ModifiedBy > 0;
        }
    }
}