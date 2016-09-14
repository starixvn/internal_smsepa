namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class DeleteDepartmentFlowCriteria : CriteriaBase
    {
        public int DepartmentId { get; set; }

        public int FlowTemplateId { get; set; }

        public bool IsGroup { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return DepartmentId > 0 && FlowTemplateId > 0 && ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "SettingDepartmentFlow_Delete";
        }
    }
}