namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetAllTargetEmployeeSettingCriteria : CriteriaBase
    {
        public int? GroupId { get; set; }

        public int? EmployeeId { get; set; }

        public int? TargetTypeId { get; set; }

        public bool ShowAll { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaTargetTemplateEmployee_Get";
        }
    }
}