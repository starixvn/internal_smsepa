// -----------------------------------------------------------------------
// <copyright file="MenuItemModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.31">Created</change>
// </history>

namespace SMS.EPA.Model.Employee
{
    public class MenuItemModel
    {
        public int MenuId { get; set; }

        public string MenuName { get; set; }

        public string MenuLink { get; set; }

        public int? ParentId { get; set; }

        public int FunctionId { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; }
    }
}