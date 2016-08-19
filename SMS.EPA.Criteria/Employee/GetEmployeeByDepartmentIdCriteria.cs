namespace SMS.EPA.Criteria
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