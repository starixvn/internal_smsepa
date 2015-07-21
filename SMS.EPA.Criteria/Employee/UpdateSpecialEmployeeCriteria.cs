// ----------------------------------------------------------------------
// <copyright file="UpdateSpecialEmployeeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/7/2015 4:42:33 PM">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Employee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Update special employee criteria.
    /// </summary>
    public class UpdateSpecialEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string Code { get; set; }

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

        public int ModifiedBy { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0 && !string.IsNullOrWhiteSpace(Code) && !string.IsNullOrWhiteSpace(NickName)
                && !string.IsNullOrWhiteSpace(Username) && Username.IndexOf(' ') == -1;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_Update";
        }
    }
}
