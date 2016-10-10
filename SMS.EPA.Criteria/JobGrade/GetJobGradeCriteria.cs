namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetJobGradeCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override string GetSettingKey()
        {
            return "JobGrade_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}