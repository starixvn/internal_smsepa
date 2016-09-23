namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEPAByEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "EPA_GetByEmployee";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}