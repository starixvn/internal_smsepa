// ----------------------------------------------------------------------
// <copyright file="IgnoredViolationModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/16/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.AttendanceSummary
{
    using System;

    public class IgnoredViolationModel
    {
        public DateTime WorkDate { get; set; }

        public int EmployeeId { get; set; }

        public int ViolationId { get; set; }

        public bool Ignored { get; set; }
    }
}
