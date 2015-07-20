// -----------------------------------------------------------------------
// <copyright file="ProcessType.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.20">Created</change>
// </history>

namespace SMS.EPA.Core.Enumerations
{
    using System.ComponentModel;

    public enum ProcessType : int
    {
        None = 0,

        [Description("Admin")]
        Admin = 1,

        [Description("Self")]
        Self = 2,

        [Description("Evaluator")]
        Evaluator = 3,

        [Description("Reviewer")]
        Reviewer = 4,

        [Description("Management")]
        Management = 5,

        [Description("HumanResource")]
        HumanResource = 6
    }
}