namespace SMS.EPA.Criteria.Authentication
{
    using Fanex.Data.Repository;

    public class GetEmployeeByDomainUsernameCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public string Username { get; set; }

        public string DomainUsername
        {
            get
            {
                return Username.Contains("sms") ? Username : string.Format("sms\\{0}", Username);
            }
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(DomainUsername);
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_GetByDomainUsername";
        }
    }
}