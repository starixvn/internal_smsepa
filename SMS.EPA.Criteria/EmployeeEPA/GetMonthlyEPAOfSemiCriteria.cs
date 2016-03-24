namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetMonthlyEPAOfSemiCriteria : CriteriaBase
    {
        public string EPAIds { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrEmpty(EPAIds);
        }

        public override string GetSettingKey()
        {
            return "EPA_SemiAnnual_GetMonthly";
        }
    }
}