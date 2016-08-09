namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class GetEmployeeLoginCriteria : CriteriaBase
    {
        public string Code { get; set; }

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
            return !string.IsNullOrWhiteSpace(Code) || !string.IsNullOrWhiteSpace(DomainUsername);
        }

        public override string GetSettingKey()
        {
            return "Login_GetEmployee";
        }
    }
}