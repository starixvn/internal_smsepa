// ----------------------------------------------------------------------
// <copyright file="GetEmployeeByCodeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 12:55:28 PM">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Authentication
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get employee by code.
    /// </summary>
    public class GetEmployeeByCodeCriteria : CriteriaBase
    {
        public string Code { get; set; }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(Code);
        }

        public override string GetSettingKey()
        {
            return "EpaEmployee_GetByCode";
        }
    }
}
