// ----------------------------------------------------------------------
// <copyright file="MailMode.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/04/2015">Created.</change>
// </history>

namespace SMS.EPA.Core.Enumerations
{
    public enum MailMode : int
    {
        /// <summary>
        /// None item.
        /// </summary>
        None = 0,

        /// <summary>
        /// Start mail item.
        /// </summary>
        Start = 1,

        /// <summary>
        /// Reminder item.
        /// </summary>
        Reminder = 2,

        /// <summary>
        /// Notify change item.
        /// </summary>
        NotifyChange = 3
    }
}
