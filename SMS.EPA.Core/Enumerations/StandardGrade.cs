// ----------------------------------------------------------------------
// <copyright file="StandardGrade.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="8/28/2015">Created.</change>
// </history>

namespace SMS.EPA.Core.Enumerations
{
    using System.ComponentModel;

    public enum StandardGrade : int
    {
        None = 0,

        [Description("A")]
        Excellent = 1,

        [Description("B")]
        Good = 2,

        [Description("C")]
        Standard = 3,

        [Description("D")]
        UnderPerformance = 4,

        [Description("E")]
        Fail = 5
    }
}
