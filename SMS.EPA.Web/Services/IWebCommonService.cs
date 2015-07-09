// ----------------------------------------------------------------------
// <copyright file="IWebCommonService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/9/2015 5:39:22 PM">Created.</change>
// </history>

namespace SMS.EPA.Web.Services
{
    using SMS.EPA.Core.Enumerations;

    /// <summary>
    /// The interface of web common service.
    /// </summary>
    public interface IWebCommonService
    {
        int GetPermission(string functionKey);

        bool HasPermissionOnFunction(string functionKey, Policy policy);
    }
}
