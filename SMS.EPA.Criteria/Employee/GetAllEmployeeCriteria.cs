namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetAllEmployeeCriteria : CriteriaBase
    {
        public string Query { get; set; }

        public int? GroupId { get; set; }

        public bool? IsLocal { get; set; }

        public bool? IsSpecialUser { get; set; }

        public bool? IsDisabledUser { get; set; }

        public bool? HasMonthlyEPA { get; set; }

        public int? RoleId { get; set; }

        public int? ApproverId { get; set; }

        public bool? IncludeVerifier { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_Get";
        }
    }
}