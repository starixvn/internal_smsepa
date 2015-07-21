// ----------------------------------------------------------------------
// <copyright file="GetAllEmployeeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/7/2015 10:14:18 AM">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Employee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get all employee criteria.
    /// </summary>
    public class GetAllEmployeeCriteria : CriteriaBase
    {
        public string Query { get; set; }

        public int? GroupId { get; set; }

        public bool? IsSpecialUser { get; set; }

        public bool? IsDisabledUser { get; set; }

        public int? RoleId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_Get";
        }
    }
}
