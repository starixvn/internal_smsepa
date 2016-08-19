namespace SMS.EPA.Criteria
{
    using System;
    using Fanex.Data.Repository;

    public class ChangePasswordCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string NewPassword { get; set; }

        [SpParam(Ignored = true)]
        public string OldPassword { get; set; }

        [SpParam(Ignored = true)]
        public string ConfirmPassword { get; set; }

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0
                && !string.IsNullOrWhiteSpace(NewPassword)
                && !string.IsNullOrWhiteSpace(OldPassword)
                && !string.IsNullOrWhiteSpace(ConfirmPassword)
                && NewPassword.Equals(ConfirmPassword, StringComparison.OrdinalIgnoreCase);
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_ChangePassword";
        }
    }
}