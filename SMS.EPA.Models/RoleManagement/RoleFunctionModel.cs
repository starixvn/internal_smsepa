// ----------------------------------------------------------------------
// <copyright file="RoleFunctionModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/9/2015 1:54:03 PM">Created.</change>
// </history>

namespace SMS.EPA.Model.RoleManagement
{
    using System.Collections.Generic;

    /// <summary>
    /// The role function model.
    /// </summary>
    public class RoleFunctionModel : FunctionModel
    {
        public RoleFunctionModel()
        {
            ChildFunctions = new List<RoleFunctionModel>();
        }

        public IEnumerable<RoleFunctionModel> ChildFunctions { get; set; }
    }
}
