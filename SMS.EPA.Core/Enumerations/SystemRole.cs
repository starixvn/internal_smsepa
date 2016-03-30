// ----------------------------------------------------------------------
// <copyright file="SystemRole.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/9/2015 2:28:00 PM">Created.</change>
// </history>

namespace SMS.EPA.Core.Enumerations
{
    using System.ComponentModel;

    public enum SystemRole : int
    {
        None = 0,

        [Description("SA")]
        SystemAdmin = 1,

        [Description("HR")]
        HumanResource = 2,

        [Description("MGT")]
        Management = 3,

        [Description("Reviewer")]
        Reviewer = 4,

        [Description("Evaluator")]
        Evaluator = 5,

        [Description("Employee")]
        Employee = 6,

        [Description("Approver")]
        Approver = 7,

        [Description("Section Admin")]
        SectionAdmin = 8
    }
}
