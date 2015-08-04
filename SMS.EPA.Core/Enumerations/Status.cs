// -----------------------------------------------------------------------
// <copyright file="Status.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.03.08">Created</change>
// </history>

namespace SMS.EPA.Core.Enumerations
{
    using System.ComponentModel;

    public enum Status
    {
        [Description("Open")]
        Open = 0,

        [Description("Closed")]
        Closed = 1
    }
}