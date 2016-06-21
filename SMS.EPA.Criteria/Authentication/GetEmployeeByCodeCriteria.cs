namespace SMS.EPA.Criteria.Authentication
{
    using Fanex.Data.Repository;

    public class GetEmployeeByCodeCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Code);
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_GetByCode";
        }
    }
}