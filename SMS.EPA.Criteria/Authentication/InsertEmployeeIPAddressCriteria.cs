// ----------------------------------------------------------------------
// <copyright file="InsertEmployeeIPAddressCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="11/4/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Authentication
{
    using Fanex.Data.Repository;

    public class InsertEmployeeIPAddressCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public string IPAddress { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployeeIpAddress_Insert";
        }
    }
}
