namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class AssignTargetTemplateToEmployeeCriteria : CriteriaBase
    {
        public int TargetTemplateId { get; set; }

        public string EmployeeIds { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return TargetTemplateId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaTargetTemplateEmployee_Assign";
        }
    }
}