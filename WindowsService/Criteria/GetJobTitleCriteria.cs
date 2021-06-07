namespace SMS.EPA.WindowsService.Criteria
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