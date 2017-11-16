namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEmployeeFlowSettingCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public int? DepartmentId { get; set; }

        public int? GroupId { get; set; }

        public bool? IsMonthlyPeriod { get; set; }

        public override bool IsValid() => FlowTemplateId > 0;

        public override string GetSettingKey() => "EpaEmployeeFlowSetting_Get";
    }
}