namespace SMS.EPA.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class GetEmployeeByDepartmentIdCriteria : CriteriaBase
    {
        public int? DepartmentId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_GetByDepartmentId";
        }
    }
}