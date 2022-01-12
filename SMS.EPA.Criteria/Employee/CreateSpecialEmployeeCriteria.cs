namespace SMS.EPA.Criteria
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CreateSpecialEmployeeCriteria : CriteriaBase
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

        public string Password { get; set; }

        public bool Enabled { get; set; }

        public bool IsLocal { get; set; }

        public string Email { get; set; }

        public int CreatedBy { get; set; }

        public override bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(NickName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(Username) || Username.Contains(" "))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(FullName))
            {
                return false;
            }

            if (string.IsNullOrWhiteSpace(Password))
            {
                return false;
            }

            return CreatedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_Insert";
        }
    }
}