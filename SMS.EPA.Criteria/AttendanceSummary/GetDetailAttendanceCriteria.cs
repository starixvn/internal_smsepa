// ----------------------------------------------------------------------
// <copyright file="GetDetailAttendanceCriteria.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/14/2015">Created.</change>
// </history>

namespace SMS.EPA.Criteria.AttendanceSummary
{
    using System;
    using Fanex.Data.Repository;

    public class GetDetailAttendanceCriteria : CriteriaBase
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public int? EmployeeId { get; set; }

        public int? DepartmentId { get; set; }

        public int? ReasonId { get; set; }

        public override bool IsValid()
        {
            return StartDate <= EndDate;
        }

        public override string GetSettingKey()
        {
            return string.Empty;
        }
    }
}
