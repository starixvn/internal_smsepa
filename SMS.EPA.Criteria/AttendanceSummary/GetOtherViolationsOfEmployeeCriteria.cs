// ----------------------------------------------------------------------
// <copyright file="GetOtherViolationsOfEmployeeCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/11/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.AttendanceSummary
{
    using System;
    using Fanex.Data.Repository;

    public class GetOtherViolationsOfEmployeeCriteria : CriteriaBase
    {
        public int EmployeeId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public DateTime? WorkDate { get; set; }

        public override bool IsValid()
        {
            return EmployeeId > 0 && StartDate <= EndDate;
        }

        public override string GetSettingKey()
        {
            return "EpaAttendanceSummary_GetOtherViolations";
        }
    }
}
