// ----------------------------------------------------------------------
// <copyright file="GetAttendanceSummaryCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 12:55:28 PM">Created.</change>
// </history>

namespace SMS.EPA.Criteria.AttendanceSummary
{
    using System;
    using Fanex.Data.Repository;

    public class GetAttendanceSummaryCriteria : CriteriaBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int ApproverId { get; set; }

        public int? DepartmentId { get; set; }

        public int? EmployeeId { get; set; }

        public int? ReasonId { get; set; }

        public override bool IsValid()
        {
            return EndDate >= StartDate && ApproverId > 0;
        }

        public override string GetSettingKey()
        {
            return "EpaAttendanceSummary_Get";
        }
    }
}
