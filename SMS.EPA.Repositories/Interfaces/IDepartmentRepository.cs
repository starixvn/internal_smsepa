namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Department;

    public interface IDepartmentRepository
    {
        IEnumerable<DepartmentModel> GetDepartment(ICriteria criteria);
    }
}