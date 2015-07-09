// ----------------------------------------------------------------------
// <copyright file="FunctionModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="7/9/2015 1:52:58 PM">Created.</change>
// </history>

namespace SMS.EPA.Model.RoleManagement
{
    /// <summary>
    /// The function model.
    /// </summary>
    public class FunctionModel
    {
        public int FunctionId { get; set; }

        public string FunctionKey { get; set; }

        public string FunctionName { get; set; }

        public int? ParentFunctionId { get; set; }

        public bool IsReadOnly { get; set; }

        public bool IsRead { get; set; }

        public bool IsHost { get; set; }
    }
}
