namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEPAInfoByManagementCriteria : CriteriaBase
    {
        public int EPAId { get; set; }

        public int ManagementId { get; set; }

        public override bool IsValid()
        {
            return EPAId > 0 && ManagementId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_GetInfoByManagement";
        }
    }
}