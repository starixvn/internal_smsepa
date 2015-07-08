// ----------------------------------------------------------------------
// <copyright file="GetAllJobTitleCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/8/2015 9:53:36 AM">Created.</change>
// </history>

namespace SMS.EPA.Criteria.Employee
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get all job title criteria.
    /// </summary>
    public class GetAllJobTitleCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaJobTitle_Get";
        }
    }
}
