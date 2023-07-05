namespace SMS.EPA.Criteria.Dashboard
{
    using Fanex.Data.Repository;

    public class GetJobLevelStandardDashboardCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "JobLevelStandard_GetDashboard";

        public override bool IsValid() => EmployeeId > 0;
    }
}
