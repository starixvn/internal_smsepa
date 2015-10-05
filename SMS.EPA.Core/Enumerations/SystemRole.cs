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

    /// <summary>
    /// The system role ENUM.
    /// </summary>
    public enum SystemRole : int
    {
        /// <summary>
        /// None item.
        /// </summary>
        None = 0,

        /// <summary>
        /// System Admin item.
        /// </summary>
        [Description("SA")]
        SystemAdmin = 1,

        /// <summary>
        /// Human Resource item.
        /// </summary>
        [Description("HR")]
        HumanResource = 2,

        /// <summary>
        /// Management item.
        /// </summary>
        [Description("MGT")]
        Management = 3,

        /// <summary>
        /// Reviewer item.
        /// </summary>
        [Description("Reviewer")]
        Reviewer = 4,

        /// <summary>
        /// Evaluator item.
        /// </summary>
        [Description("Evaluator")]
        Evaluator = 5,

        /// <summary>
        /// Employee item.
        /// </summary>
        [Description("Employee")]
        Employee = 6,

        /// <summary>
        /// Approver item.
        /// </summary>
        [Description("Approver")]
        Approver = 7
    }
}
