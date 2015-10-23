// ----------------------------------------------------------------------
// <copyright file="GetOtherViolationTypeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="10/23/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.AttendanceSummary
{
    using Fanex.Data.Repository;

    public class GetOtherViolationTypeCriteria : CriteriaBase
    {
        public bool? IsActive { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "OtherViolationType_Get";
        }
    }
}
