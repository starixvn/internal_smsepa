// ----------------------------------------------------------------------
// <copyright file="ViolationModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/14/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.AttendanceSummary
{
    public class ViolationModel
    {
        public int ViolationId { get; set; }

        public string Name { get; set; }

        public float Score { get; set; }

        public string Value { get; set; }

        public bool HasViolated { get; set; }

        public bool HasIgnored { get; set; }
    }
}
