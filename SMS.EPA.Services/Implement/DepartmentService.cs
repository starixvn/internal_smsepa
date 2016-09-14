namespace SMS.EPA.Services.Implement
{
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Department;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;

    public class DepartmentService : BaseService, IDepartmentService
    {
        private readonly IDepartmentRepository departmentRepository;

        public DepartmentService()
            : this(null)
        {
        }

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            this.departmentRepository = departmentRepository ?? new DepartmentRepository();
        }

        public IList<Department> GetDepartment(GetDepartmentCriteria criteria)
        {
            List<Department> departments = null;

            if (criteria != null && criteria.IsValid())
            {
                departments = departmentRepository.GetDepartment(criteria).ToList();
                if (departments.Any())
                {
                    departments.ForEach(x => x.Childrens.AddRange(departments.Where(y => y.ParentId == x.DepartmentId)));
                }
            }

            return departments ?? new List<Department>();
        }
    }
}