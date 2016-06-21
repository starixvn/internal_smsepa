namespace SMS.EPA.Criteria.Employee
{
    using Fanex.Data.Repository;

    public class UpdateSpecialEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

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

        public bool Enabled { get; set; }

        public bool IsLocal { get; set; }

        public string Email { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0
                && !string.IsNullOrWhiteSpace(NickName)
                && !string.IsNullOrWhiteSpace(Username)
                && Username.IndexOf(' ') == -1;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_Update";
        }
    }
}