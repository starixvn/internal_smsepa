namespace SMS.EPA.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetEmployeeByActorIdCriteria : CriteriaBase
    {
        public int ActorId { get; set; }

        public string PeriodIds { get; set; }

        public int? DepartmentId { get; set; }

        public bool? IsLocal { get; set; }

        public string Query { get; set; }

        public override string GetSettingKey()
        {
            return "Report_GetEmployees";
        }

        public override bool IsValid()
        {
            return ActorId > 0;
        }
    }
}