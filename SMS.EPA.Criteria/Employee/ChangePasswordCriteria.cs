// ----------------------------------------------------------------------
// <copyright file="ChangePasswordCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/8/2015 4:51:56 PM">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Employee
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Change password criteria.
    /// </summary>
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
