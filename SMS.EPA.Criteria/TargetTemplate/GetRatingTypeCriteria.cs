namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetRatingTypeCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaRatingType_Get";
        }
    }
}