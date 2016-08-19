namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEmployeeByGroupIdCriteria : CriteriaBase
    {
        public int GroupId { get; set; }

        public override bool IsValid()
        {
            return GroupId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_GetByGroupId";
        }
    }
}