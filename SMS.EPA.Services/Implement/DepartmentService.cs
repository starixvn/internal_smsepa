namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model.Enitites;

    public class DepartmentService : IDepartmentService
    {
        private readonly IBaseService baseService;

        public DepartmentService(IBaseService baseService = null)
        {
            this.baseService = baseService ?? new BaseService();
        }

        public IList<MultiSelectItem> GetOrgChart()
        {
            var criteria = new GetOrgChartCriteria();
            var tuple = baseService.FetchMultiple<Department, Section, Team>(criteria);

            var departments = tuple.Item1.ToList();
            var sections = tuple.Item2.ToList();
            var teams = tuple.Item3.ToList();

            return GetOrgChart(departments, sections, teams);
        }

        private IList<MultiSelectItem> GetOrgChart(IList<Department> departments, IList<Section> sections, IList<Team> teams)
        {
            return departments.Select(dept => new MultiSelectItem
            {
                Id = dept.DepartmentId,
                Text = dept.Name,
                ParentId = dept.ParentId,
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
    }
}