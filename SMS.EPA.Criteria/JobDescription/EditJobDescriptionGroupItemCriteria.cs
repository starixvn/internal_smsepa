namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class EditJobDescriptionGroupItemCriteria : CriteriaBase
    {
        public int? GroupId { get; set; }
        public int? EmployeeId { get; set; }
        public int GroupItemId { get; set; }
        public string Name { get; set; }

        public override string GetSettingKey() => "JobDescription_UpdateGroupItem";

        public override bool IsValid() => (GroupId.HasValue || EmployeeId.HasValue) && GroupItemId > 0 && !string.IsNullOrWhiteSpace(Name);
    }
}