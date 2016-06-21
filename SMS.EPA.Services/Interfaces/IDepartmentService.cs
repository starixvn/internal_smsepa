namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria.Department;
    using SMS.EPA.Model.Department;

    public interface IDepartmentService
    {
        List<DepartmentModel> GetDepartment(GetDepartmentCriteria criteria);
    }
}