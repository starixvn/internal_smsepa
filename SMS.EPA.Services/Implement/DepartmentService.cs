namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Department;
    using SMS.EPA.Model.Department;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

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

        public List<DepartmentModel> GetDepartment(GetDepartmentCriteria criteria)
        {
            List<DepartmentModel> departments = new List<DepartmentModel>();

            if (criteria != null && criteria.IsValid())
            {
                departments = _departmentRepository.GetDepartment(criteria);
            }

            return departments;
        }
    }
}