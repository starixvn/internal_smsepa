namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;

    public class MenuService : BaseService, IMenuService
    {
        public IEnumerable<MenuGroupModel> GetMenusByEmployeeId(int employeeId)
        {
            var criteria = new GetMenusByEmployeeIdCriteria
            {
                EmployeeId = employeeId
            };
            IEnumerable<MenuGroupModel> menus = new List<MenuGroupModel>();
            if (criteria.IsValid())
            {
                IEnumerable<MenuItemModel> menuItems = GetData<MenuItemModel>(criteria);

                menus = GroupMenuItem(menuItems);
            }

            return menus;
        }

        private IEnumerable<MenuGroupModel> GroupMenuItem(IEnumerable<MenuItemModel> menuItems)
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
                    .OrderBy(menuItem => menuItem.DisplayOrder);

                menuGroups.Add(menuGroupItem);
            }

            return menuGroups;
        }
    }
}