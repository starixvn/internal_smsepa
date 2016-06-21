namespace SMS.EPA.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Authentication;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.RoleManagement;
    using SMS.EPA.Repositories.Interfaces;

    public class EmployeeRepository : IEmployeeRepository
    {
        public RawEmployeeModel GetAllEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    IEnumerable<EmployeeModel> employees = multi.Read<EmployeeModel, JobTitle, Group, EmployeeModel>(
                           (emp, jb, gr) =>
                           {
                               emp.JobTitle = jb;
                               emp.Group = gr;

                               return emp;
                           },
                           "JobTitleId, GroupId");

                    IEnumerable<RawEmployeeRoleModel> roles = multi.Read<RawEmployeeRoleModel>();

                    var rawEmployee = new RawEmployeeModel
                    {
                        Employees = employees,
                        Roles = roles
                    };

                    return rawEmployee;
                }
            }
        }

        public UserSession GetEmployee(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    UserSession userSession = null;
                    IEnumerable<EmployeeModel> employees = multi.Read<EmployeeModel, JobTitle, DepartmentModel, Group, EmployeeModel>(
                           (emp, jb, dept, gr) =>
                           {
                               emp.JobTitle = jb;
                               emp.Department = dept;
                               emp.Group = gr;
                               return emp;
                           },
                           "JobTitleId, DepartmentId, GroupId");

                    if (employees != null && employees.Any())
                    {
                        EmployeeModel employee = employees.First();
                        IEnumerable<SystemRoleModel> roles = multi.Read<SystemRoleModel>();
                        IEnumerable<FunctionModel> functions = multi.Read<FunctionModel>();
                        IEnumerable<MenuItemModel> menuItems = multi.Read<MenuItemModel>();

                        userSession = new UserSession
                        {
                            EmployeeInfo = employee,
                            Roles = roles,
                            Functions = functions,
                            Menus = GroupMenuItem(menuItems)
                        };
                    }

                    return userSession;
                }
            }
        }

        public EmployeeModel GetEmployeeById(ICriteria criteria)
        {
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (GridReader multi = db.QueryMultiple(criteria.GetSpParams()))
                {
                    EmployeeModel employee = multi.Read<EmployeeModel, JobTitle, DepartmentModel, Group, EmployeeModel>(
                           (emp, jb, dept, gr) =>
                           {
                               emp.JobTitle = jb;
                               emp.Department = dept;
                               emp.Group = gr;
                               return emp;
                           },
                           "JobTitleId, DepartmentId, GroupId").FirstOrDefault();

                    if (employee != null)
                    {
                        employee.Roles = multi.Read<SystemRoleModel>().ToList();
                    }

                    return employee;
                }
            }
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