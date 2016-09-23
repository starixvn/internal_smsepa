namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEPAToUpdateFinalScoreCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int? GroupId { get; set; }

        public int? EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_GetEPAToUpdateFinalScore";
        }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }
    }
}