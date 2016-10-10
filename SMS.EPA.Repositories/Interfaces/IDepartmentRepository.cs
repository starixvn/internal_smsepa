namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Enitites;

    public interface IDepartmentRepository
    {
        IEnumerable<Department> GetDepartment(ICriteria criteria);
    }
}