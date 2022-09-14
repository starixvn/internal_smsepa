namespace SMS.EPA.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class GetEmployeeCriteria : CriteriaBase
    {
        public string Query { get; set; }

        [SpParam(SqlParamName = "GroupIds")]
        public string TeamIds { get; set; }

        public bool? IsLocal { get; set; }
        public bool? IsSpecialUser { get; set; }
        public bool? IsDisabledUser { get; set; }
        public bool? HasMonthlyEPA { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Employee_Get";
    }
}