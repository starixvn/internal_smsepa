namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class UpdateHasMonthlyEPACriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public bool HasMonthlyEPA { get; set; }

        public int ModifiedBy { get; set; }

        public override string GetSettingKey()
        {
            return "EpaEmployee_UpdateHasMonthlyEPA";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}