namespace SMS.EPA.WindowsService.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class GetDepartmentCriteria : CriteriaBase
    {
        public DateTime? FromDate { get; set; }

        public override string GetSettingKey()
        {
            return "Sync_Department";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}