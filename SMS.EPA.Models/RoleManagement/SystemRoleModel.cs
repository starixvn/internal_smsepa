// ----------------------------------------------------------------------
// <copyright file="SystemRoleModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/6/2015 10:54:20 AM">Created.</change>
// </history>

namespace SMS.EPA.Model.RoleManagement
{
    using System;

    [Serializable]
    public class SystemRoleModel
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; }

        public string RoleCode { get; set; }
    }
}
