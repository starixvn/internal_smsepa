namespace SMS.EPA.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class SearchEmployeeCriteria : CriteriaBase
    {
        public string Query { get; set; }

        [SpParam(SqlParamName = "GroupIds")]
        public string TeamIds { get; set; }

        public bool? IsLocal { get; set; }
        public bool? IsSpecialUser { get; set; }
        public bool? IsDisabledUser { get; set; }
        public int? ApproverId { get; set; }
        public bool? IncludeVerifier { get; set; }

        public override bool IsValid() => true;

        public override string GetSettingKey() => "Employee_Search";
    }
}