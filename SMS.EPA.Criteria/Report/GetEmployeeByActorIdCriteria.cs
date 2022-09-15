namespace SMS.EPA.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetEmployeeByActorIdCriteria : CriteriaBase
    {
        public int ActorId { get; set; }
        public string PeriodIds { get; set; }
        public string TeamIds { get; set; }
        public bool? IsLocal { get; set; }
        public string Query { get; set; }

        public override string GetSettingKey() => "Report_GetEmployees";

        public override bool IsValid() => ActorId > 0;
    }
}