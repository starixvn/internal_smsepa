namespace SMS.EPA.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetEPAToUpdateFinalScoreCriteria : CriteriaBase
    {
        public int PeriodId { get; set; }

        public int? DepartmentId { get; set; }

        public bool? IsLocal { get; set; }

        public int? EmployeeId { get; set; }

        [SpParam(Ignored = true)]
        public string PeriodName { get; set; }

        [SpParam(Ignored = true)]
        public string EmployeeIds { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_GetEPAToUpdateFinalScore_xtest";
        }

        public override bool IsValid()
        {
            return PeriodId > 0;
        }
    }
}