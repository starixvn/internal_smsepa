namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetDataToUpdateFinalScoreCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }
        public string EmployeeIds { get; set; }

        public override bool IsValid() => PeriodId > 0;

        public override string GetSettingKey() => "EPA_GetDataToUpdateFinalScore";
    }
}