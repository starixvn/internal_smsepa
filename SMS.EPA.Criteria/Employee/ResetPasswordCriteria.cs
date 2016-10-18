namespace SMS.EPA.Criteria
{
    using Fanex.Data.Repository;

    public class ResetPasswordCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string NewPassword { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0 && !string.IsNullOrWhiteSpace(NewPassword);
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_ChangePassword";
        }
    }
}