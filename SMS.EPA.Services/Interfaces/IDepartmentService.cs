namespace SMS.EPA.Services.Interfaces
{
    using SMS.EPA.Criteria.Department;
    using SMS.EPA.Model.Enitites;
    using System.Collections.Generic;

    public interface IDepartmentService
    {
        IList<Department> GetDepartment(GetDepartmentCriteria criteria);
    }
}