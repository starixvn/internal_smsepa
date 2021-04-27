namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class GetEmployeeSettingJobDescriptionCriteria : CriteriaBase
    {
        public int? GroupId { get; set; }
        public int? EmployeeId { get; set; }
        public bool ShowAll { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "JobDescription_GetEmployeeSetting";
    }
}