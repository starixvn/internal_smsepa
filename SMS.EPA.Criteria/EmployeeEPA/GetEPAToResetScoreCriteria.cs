namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetEPAToResetScoreCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public string NickName { get; set; }

        public override bool IsValid()
        {
            return PeriodId > 0 && string.IsNullOrEmpty(NickName);
        }

        public override string GetSettingKey()
        {
            return "EPA_GetEPAToResetScore";
        }
    }
}