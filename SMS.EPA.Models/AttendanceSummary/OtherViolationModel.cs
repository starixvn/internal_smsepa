// ----------------------------------------------------------------------
// <copyright file="OtherViolationModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/11/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.AttendanceSummary
{
    using System;

    public class OtherViolationModel
    {
        public DateTime WorkDate { get; set; }

        public int EmployeeId { get; set; }

        public float Score { get; set; }

        public string Name { get; set; }

        public string Comment { get; set; }
    }
}
