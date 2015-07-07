// ----------------------------------------------------------------------
// <copyright file="HashProvider.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 3:33:13 PM">Created.</change>
// </history>

namespace SMS.EPA.Core.Enumerations
{
    /// <summary>
    /// ENUM Hash Provider.
    /// </summary>
    public enum HashProvider
    {
        /// <summary>
        /// MD5 value item.
        /// </summary>
        MD5,

        /// <summary>
        /// SHA1 item.
        /// </summary>
        Sha1,

        /// <summary>
        /// SHA256 item.
        /// </summary>
        Sha256,

        /// <summary>
        /// SHA384 item.
        /// </summary>
        Sha384,

        /// <summary>
        /// SHA512 item.
        /// </summary>
        Sha512
    }
}
