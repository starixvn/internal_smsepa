namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class CreateJobDescriptionGroupItemCriteria : CriteriaBase
    {
        public int? GroupId { get; set; }
        public int? EmployeeId { get; set; }
        public string Name { get; set; }

        public override string GetSettingKey() => "JobDescription_InsertGroupItem";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(Name) && (GroupId.HasValue || EmployeeId.HasValue);
    }
}