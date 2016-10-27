namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model.Enitites;

    public interface IDepartmentService
    {
        IList<Department> GetDepartment(GetDepartmentCriteria criteria);
    }
}