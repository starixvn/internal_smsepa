namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetAllJobTitleCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaJobTitle_Get";
        }
    }
}