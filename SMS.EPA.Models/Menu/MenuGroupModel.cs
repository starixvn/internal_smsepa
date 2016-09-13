namespace SMS.EPA.Model
{
    using System.Collections.Generic;

    public class MenuGroupModel : MenuItemModel
    {
        public MenuGroupModel()
        {
            MenuItems = new List<MenuItemModel>();
        }

        public IList<MenuItemModel> MenuItems { get; set; }
    }
}