namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model.Enitites;

    public class UnitService : IUnitService
    {
        private readonly IBaseService baseService;
        private readonly IDepartmentService departmentService;

        public UnitService(
            IBaseService baseService = null,
            IDepartmentService departmentService = null)
        {
            this.baseService = baseService ?? new BaseService();
            this.departmentService = departmentService ?? new DepartmentService();
        }

        public IList<Unit> Get(bool? isActive = true)
        {
            var criteria = new GetUnitCriteria
            {
                IsActive = isActive
            };

            return baseService.GetData<Unit>(criteria).ToList();
        }

        public IList<MultiSelectItem> GetOrgChart()
        {
            var criteria = new GetUnitCriteria
            {
                IsActive = true
            };
            var units = baseService.GetData<Unit>(criteria).ToList();
            var items = departmentService.GetOrgChart().ToList();

            if (items.Exists(x => x.ParentId == 0))
            {
                units.Add(new Unit());
            }

            return units.Select(x => new MultiSelectItem
            {
                Id = x.UnitId,
                Text = x.Name,
                Items = items.Where(dept => dept.ParentId == x.UnitId).ToList()
            }).ToList();
        }
    }
}