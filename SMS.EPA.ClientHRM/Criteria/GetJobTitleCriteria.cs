namespace SMS.EPA.ClientHrm.Criteria
{
    using Fanex.Data.Repository;

    public class GetJobTitleCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "JobTitle_Get";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}