namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetMenusByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "Menu_GetByEmployeeId";
        }
    }
}