namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class UpdateEmployeeSettingGroupItemCriteria : CriteriaBase
    {
        public int GroupItemId { get; set; }

        public bool IsSelected { get; set; }

        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "JobDescription_UpdateEmployeeSettingGroupItem";

        public override bool IsValid() => GroupItemId > 0 && EmployeeId > 0;
    }
}