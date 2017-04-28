namespace SMS.EPA.Criteria
{
    using System;
    using Fanex.Data;
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

        private bool IsPasswordValid()
        {
            return !string.IsNullOrEmpty(NewPassword)
                   && !string.IsNullOrEmpty(OldPassword);
        }

        private bool IsConfirmEqualNewPassword()
        {
            return NewPassword.Equals(ConfirmPassword, StringComparison.OrdinalIgnoreCase);
        }

        public override bool IsValid()
        {
            return EmployeeId > 0
                   && IsPasswordValid()
                   && IsConfirmEqualNewPassword()
                   && ModifiedBy > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_ChangePassword";
        }
    }
}