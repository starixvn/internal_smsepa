namespace SMS.EPA.Criteria.EmployeeEPA
{
    using Fanex.Data.Repository;

    public class GetMonthlyEPAIdOfSemiCriteria : CriteriaBase
    {
        public int EPAId { get; set; }

        public override bool IsValid()
        {
            return EPAId > 0;
        }

        public override string GetSettingKey()
        {
            return "EPA_SemiAnnual_GetMonthly";
        }
    }
}