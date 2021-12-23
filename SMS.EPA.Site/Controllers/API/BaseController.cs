namespace SMS.EPA.Site.Controllers.API
{
    using SMS.EPA.Model.Enitites;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Security;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Session;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;

    [WebApiValidateAntiForgeryToken]
    [CustomActionFilter]
    public class BaseController : ApiController
    {
        private IUserControlSession userControlSession;

        public BaseController()
            : this(null)
        {
        }

        public BaseController(IUserControlSession userControlSession)
        {
            this.userControlSession = userControlSession ?? new UserControlSession();
        }

        public IUserControlSession UserControlSession
        {
            get
            {
                if (userControlSession == null || userControlSession.UserId <= 0)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }

                return userControlSession;
            }

            set
            {
                userControlSession = value;
            }
        }

        protected int CurrentEmployeeId => UserControlSession.Get<int>(Constants.CurrentEmployeeIdKey);

        protected byte GetPermissionOnFunction(string functionKey)
        {
            byte policy = (byte)Policy.None;
            if (UserControlSession.HasPermissionOnFunction(EPAFunctionKey.FlowManagement, Policy.Host))
            {
                policy = (byte)Policy.Host;
            }
            else
            {
                if (UserControlSession.HasPermissionOnFunction(EPAFunctionKey.FlowManagement, Policy.Read))
                {
                    policy = (byte)Policy.Read;
                }
            }

            return policy;
        }

        protected IList<MultiSelectItem> GetOrgChart(IList<Department> departments, IList<Section> sections, IList<Team> teams)
        {
            IList<MultiSelectItem> items;
            if (departments.Any())
            {
                items = departments.Select(dept => new MultiSelectItem
                {
                    Id = dept.DepartmentId,
                    Text = dept.Name,
                    Items = sections.Where(s => s.DepartmentId.HasValue && s.DepartmentId.Value == dept.DepartmentId).Select(s => new MultiSelectItem
                    {
                        Id = s.SectionId,
                        Text = s.Name,
                        ParentId = dept.DepartmentId,
                        Items = teams.Where(t => t.SectionId.HasValue && t.SectionId.Value == s.SectionId).Select(t => new MultiSelectItem
                        {
                            Id = t.TeamId,
                            Text = t.Name,
                            IsLeaf = true,
                            ParentId = s.SectionId
                        }).ToList()
                    }).ToList()
                }).ToList();
            }
            else if (sections.Any())
            {
                items = sections.Select(s => new MultiSelectItem
                {
                    Id = s.SectionId,
                    Text = s.Name,
                    Items = teams.Where(t => t.SectionId.HasValue && t.SectionId.Value == s.SectionId).Select(t => new MultiSelectItem
                    {
                        Id = t.TeamId,
                        Text = t.Name,
                        IsLeaf = true,
                        ParentId = s.SectionId
                    }).ToList()
                }).ToList();
            }
            else
            {
                items = teams.Select(t => new MultiSelectItem
                {
                    Id = t.TeamId,
                    Text = t.Name,
                    IsLeaf = true
                }).ToList();
            }

            return items;
        }
    }
}