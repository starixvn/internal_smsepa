namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetMenusByEmployeeIdCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override bool IsValid() => EmployeeId > 0;

        public override string GetSettingKey() => "Menu_GetByEmployeeId";
    }
}