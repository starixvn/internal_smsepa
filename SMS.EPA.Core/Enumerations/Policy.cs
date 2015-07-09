// ----------------------------------------------------------------------
// <copyright file="Policy.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/9/2015 5:39:55 PM">Created.</change>
// </history>

namespace SMS.EPA.Core.Enumerations
{
    using System.ComponentModel;

    /// <summary>
    /// The ENUM policy.
    /// </summary>
    public enum Policy
    {
        /// <summary>
        /// None policy.
        /// </summary>
        [Description("None")]
        None = 0,

        /// <summary>
        /// Read policy.
        /// </summary>
        [Description("Read")]
        Read = 1,

        /// <summary>
        /// Host policy.
        /// </summary>
        [Description("Host")]
        Host = 2
    }
}
