// ----------------------------------------------------------------------
// <copyright file="GetViolationTypeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/14/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.AttendanceSummary
{
    using Fanex.Data.Repository;

    public class GetViolationTypeCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "EpaAttendanceSummary_GetViolations";
        }
    }
}
