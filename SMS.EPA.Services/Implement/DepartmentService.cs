namespace SMS.EPA.Services.Implement
{
    using Core.Services;
    using Criteria;
    using Model.Enitites;
    using Repositories;
    using System.Collections.Generic;
    using System.Linq;

    using Interfaces;

    public class DepartmentService : BaseService, IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService()
            : this(null)
        {
        }

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository ?? new DepartmentRepository();
        }

        public IList<Department> GetDepartment(GetDepartmentCriteria criteria)
        {
            List<Department> departments = null;

            if (criteria != null && criteria.IsValid())
            {
                departments = _departmentRepository.GetDepartment(criteria).ToList();
                if (departments.Any())
                {
                    departments.ForEach(x => x.Childrens.AddRange(departments.Where(y => y.ParentId == x.DepartmentId)));
                }
            }

            return departments ?? new List<Department>();
        }
    }
}