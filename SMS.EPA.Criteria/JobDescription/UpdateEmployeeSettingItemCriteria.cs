namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class UpdateEmployeeSettingItemCriteria : CriteriaBase
    {
        public int ItemId { get; set; }
        public int EmployeeId { get; set; }
        public bool IsSelected { get; set; }

        public override string GetSettingKey() => "JobDescription_UpdateEmployeeSettingItem";

        public override bool IsValid() => ItemId > 0 && EmployeeId > 0;
    }
}