namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEmployeeToUpdateFinalScoreCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }
        public string TeamIds { get; set; }
        public bool? IsLocal { get; set; }
        public string Query { get; set; }

        public override string GetSettingKey() => "EPA_GetEmployeeToUpdateFinalScore";

        public override bool IsValid() => PeriodId > 0;
    }
}