namespace SMS.EPA.Criteria.EmployeeEPA
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