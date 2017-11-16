namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetToDoListCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public override string GetSettingKey() => "Dashboard_GetToDoList";

        public override bool IsValid() => EmployeeId > 0;
    }
}