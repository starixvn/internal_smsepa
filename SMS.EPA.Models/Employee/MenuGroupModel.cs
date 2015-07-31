// -----------------------------------------------------------------------
// <copyright file="MenuGroupModel.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Tim.Trinh" date="2015.07.31">Created</change>
// </history>

namespace SMS.EPA.Model.Employee
{
    using System.Collections.Generic;

    public class MenuGroupModel : MenuItemModel
    {
        public MenuGroupModel()
        {
            MenuItems = new List<MenuItemModel>();
        }

        public IEnumerable<MenuItemModel> MenuItems { get; set; }
    }
}