// ----------------------------------------------------------------------
// <copyright file="OtherViolationTypeModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/27/2015">Created.</change>
// </history>

namespace SMS.EPA.Model.AttendanceSummary
{
    public class OtherViolationTypeModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public float Value { get; set; }

        public bool IsActive { get; set; }
    }
}
