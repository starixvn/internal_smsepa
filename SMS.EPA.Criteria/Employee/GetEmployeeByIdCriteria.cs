namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEmployeeByIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey()
        {
            return "EpaEmployee_GetById";
        }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }
    }
}