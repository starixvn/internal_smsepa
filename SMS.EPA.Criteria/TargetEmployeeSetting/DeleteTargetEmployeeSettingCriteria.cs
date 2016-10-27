namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class DeleteTargetEmployeeSettingCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public int EmployeeId { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return TargetTemplateId > 0 && EmployeeId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaTargetTemplateEmployee_Delete";
        }
    }
}