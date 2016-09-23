namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetStandardGradeCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EPA_GetStandardGrade";
        }
    }
}