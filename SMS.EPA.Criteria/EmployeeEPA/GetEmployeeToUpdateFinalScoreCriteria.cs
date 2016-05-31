namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEmployeeToUpdateFinalScoreCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int? GroupId { get; set; }

        public string Query { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_GetEmployeeToUpdateFinalScore";
        }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }
    }
}