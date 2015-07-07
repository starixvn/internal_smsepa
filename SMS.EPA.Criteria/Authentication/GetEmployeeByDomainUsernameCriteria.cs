// ----------------------------------------------------------------------
// <copyright file="GetEmployeeByDomainUsernameCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 1:13:32 PM">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Authentication
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get employee by domain username criteria.
    /// </summary>
    public class GetEmployeeByDomainUsernameCriteria : CriteriaBase
    {
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
            return !string.IsNullOrWhiteSpace(DomainUsername);
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_GetByDomainUsername";
        }
    }
}
