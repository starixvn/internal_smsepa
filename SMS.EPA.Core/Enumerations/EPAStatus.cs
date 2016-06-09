// -----------------------------------------------------------------------
// <copyright file="Status.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.08.03">Created</change>
// </history>

namespace SMS.EPA.Core.Enumerations
{
    using System.ComponentModel;

    public enum EPAStatus
    {
        [Description("Open")]
        Open = 0,

        [Description("Closed")]
        Closed = 1,

        [Description("Rejected")]
        Rejected = 2
    }
}