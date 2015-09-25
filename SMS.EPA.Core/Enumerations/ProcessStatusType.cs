// ----------------------------------------------------------------------
// <copyright file="ProcessStatusType.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/25/2015">Created.</change>
// </history>

namespace SMS.EPA.Core.Enumerations
{
    using System.ComponentModel;

    public enum ProcessStatusType : int
    {
        [Description("None")]
        None = 0,

        [Description("Open")]
        Open = 1,

        [Description("In Progress")]
        InProgress = 2,

        [Description("Closed")]
        Closed = 3
    }
}
