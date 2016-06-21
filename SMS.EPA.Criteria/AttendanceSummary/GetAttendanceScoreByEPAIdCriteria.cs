namespace SMS.EPA.Criteria.AttendanceSummary
{
    using Fanex.Data.Repository;

    public class GetAttendanceScoreByEPAIdCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "EPAIds")]
        public string EvaluationIds { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(EvaluationIds);
        }

        public override string GetSettingKey()
        {
            return "EpaAttendanceSummary_GetByEPA";
        }
    }
}