namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using Criteria;
    using Model.Enitites;

    public interface IDepartmentService
    {
        IList<Department> GetDepartment(GetDepartmentCriteria criteria);
    }
}