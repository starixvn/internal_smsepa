namespace SMS.EPA.Criteria.JobDescription
{
    using Fanex.Data.Repository;

    public class GetEmployeeSettingJobDescriptionByIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override bool IsValid() => EmployeeId > 0;

        public override string GetSettingKey() => "JobDescription_GetEmployeeSettingById";
    }
}