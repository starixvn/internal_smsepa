namespace SMS.EPA.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class UpdateSpecialEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string NickName { get; set; }

        public string FullName { get; set; }

        public int GroupId { get; set; }

        public int JobTitleId { get; set; }

        public int JobLevelId { get; set; }

        [SpParam(Ignored = true)]
        public string Username { get; set; }

        public string DomainUsername => Username.Contains("sms")
            ? Username
            : $"sms\\{Username}";

        public bool Enabled { get; set; }

        public bool IsLocal { get; set; }

        public string Email { get; set; }

        public int ModifiedBy { get; set; }

        private bool IsUserNameValid()
        {
            return !string.IsNullOrWhiteSpace(Username) && !Username.Contains(" ");
        }

        private bool IsNameValid()
        {
            return !string.IsNullOrWhiteSpace(NickName) && !string.IsNullOrEmpty(FullName);
        }

        public override bool IsValid()
        {
            return EmployeeId > 0
                   && IsNameValid()
                   && IsUserNameValid()
                   && ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_Update";
        }
    }
}