namespace SMS.EPA.Site.Controllers.API
{
    using Core.Attribute;
    using Core.Services;
    using SMS.EPA.Model.Enitites;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using System.Web.Security;

    [WebApiValidateAntiForgeryToken]
    [CustomActionFilter]
    public class BaseController : ApiController
    {
        private ISessionService _sessionService;

        public BaseController()
            : this(null)
        {
        }

        public BaseController(ISessionService sessionService)
        {
            _sessionService = sessionService ?? new SessionService();
        }

        public ISessionService SessionService
        {
            get
            {
                if (_sessionService == null || _sessionService.EmployeeId <= 0)
                {
                    FormsAuthentication.RedirectToLoginPage();
                }

                return _sessionService;
            }

            set
            {
                _sessionService = value;
            }
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