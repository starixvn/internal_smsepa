namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEmployeeFlowSettingCriteria : CriteriaBase
    {
        public int FlowTemplateId { get; set; }

        public string TeamIds { get; set; }

        public bool? IsMonthly { get; set; }

        public override bool IsValid() => FlowTemplateId > 0;

        public override string GetSettingKey() => "EpaEmployeeFlowSetting_Get";
    }
}