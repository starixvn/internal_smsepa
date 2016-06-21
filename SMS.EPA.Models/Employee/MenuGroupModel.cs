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