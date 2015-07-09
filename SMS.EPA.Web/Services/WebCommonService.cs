// ----------------------------------------------------------------------
// <copyright file="WebCommonService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/9/2015 5:41:01 PM">Created.</change>
// </history>

namespace SMS.EPA.Web.Services
{
    using System.Linq;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.RoleManagement;

    /// <summary>
    /// The web common service.
    /// </summary>
    public class WebCommonService : IWebCommonService
    {
        /// <summary>
        /// Get permission on function.
        /// </summary>
        /// <param name="functionKey">The function key.</param>
        /// <returns>The permission.</returns>
        public int GetPermission(string functionKey)
        {
            int policy = (int)Policy.None;

            if (HasPermissionOnFunction(functionKey, Policy.Host))
            {
                policy = (int)Policy.Host;
            }
            else if (HasPermissionOnFunction(functionKey, Policy.Read))
            {
                policy = (int)Policy.Read;
            }

            return policy;
        }

        /// <summary>
        /// Check current user has permission on current function or not.
        /// </summary>
        /// <param name="functionKey">The function key.</param>
        /// <param name="policy">THe policy.</param>
        /// <returns>True if has permission, false: otherwise.</returns>
        public bool HasPermissionOnFunction(string functionKey, Policy policy)
        {
            FunctionModel currentFunction = SessionManager.CurrentUser.Functions
                        .Where(func => func.FunctionKey.Equals(functionKey)).FirstOrDefault();

            if (currentFunction != null)
            {
                switch (policy)
                {
                    case Policy.None:
                        return false;

                    case Policy.Read:
                        return currentFunction.IsRead || currentFunction.IsHost;

                    case Policy.Host:
                        return currentFunction.IsHost;
                }
            }

            return false;
        }
    }
}
