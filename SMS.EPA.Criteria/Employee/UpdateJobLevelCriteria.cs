namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateJobLevelCriteria : CriteriaBase
    {
        public int JobLevelId { get; set; }
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "EpaEmployee_UpdateJobLevel";

        public override bool IsValid() => EmployeeId > 0;
    }
}