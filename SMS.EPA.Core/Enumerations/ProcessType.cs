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

        [Description("Self")]
        Self = 1,

        [Description("Evaluator")]
        Evaluator = 2,

        [Description("Reviewer")]
        Reviewer = 3,

        [Description("Management")]
        Management = 4,

        [Description("Human Resource")]
        HumanResource = 5
    }
}