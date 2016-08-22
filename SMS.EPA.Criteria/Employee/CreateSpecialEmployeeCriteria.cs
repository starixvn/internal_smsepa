namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class CreateSpecialEmployeeCriteria : CriteriaBase
    {
        public string NickName { get; set; }

        public string FullName { get; set; }

        public int GroupId { get; set; }

        public int JobTitleId { get; set; }

        [SpParam(Ignored = true)]
        public string Username { get; set; }

        public string DomainUsername
        {
            get
            {
                return Username.Contains("sms") ? Username : string.Format("sms\\{0}", Username);
            }
        }

        public string Password { get; set; }

        public bool Enabled { get; set; }

        public bool IsLocal { get; set; }

        public string Email { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(NickName)
                && !string.IsNullOrEmpty(FullName)
                && !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(Password)
                && Username.IndexOf(' ') == -1
                && CreatedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_Insert";
        }
    }
}