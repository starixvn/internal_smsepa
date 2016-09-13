namespace SMS.EPA.Services.Implement
{
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class MenuService : BaseService, IMenuService
    {
        public IList<MenuGroupModel> GetMenusByEmployeeId(int employeeId)
        {
            var criteria = new GetMenusByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };

            IEnumerable<MenuItemModel> menuItems = GetData<MenuItemModel>(criteria);
            IList<MenuGroupModel> menus = GroupMenuItem(menuItems);

            return menus;
        }

       
        private IList<MenuGroupModel> GroupMenuItem(IEnumerable<MenuItemModel> menuItems)
        {
            var menuGroups = new List<MenuGroupModel>();
            IEnumerable<MenuItemModel> parentMenu = menuItems.Where(item => item.ParentId == null).OrderBy(item => item.DisplayOrder).ToList();

            foreach (MenuItemModel parent in parentMenu)
            {
                var menuGroupItem = new MenuGroupModel
                {
                    MenuId = parent.MenuId,
                    MenuName = parent.MenuName,
                    MenuLink = parent.MenuLink
                };

                menuGroupItem.MenuItems = menuItems.Where(menuItem => menuItem.ParentId == menuGroupItem.MenuId)
                    .OrderBy(menuItem => menuItem.DisplayOrder).ToList();

                menuGroups.Add(menuGroupItem);
            }

            return menuGroups;
        }
    }
}