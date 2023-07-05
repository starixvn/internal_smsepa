namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Model.Enitites;

    public interface IDepartmentRepository
    {
        IEnumerable<Department> GetDepartment(ICriteria criteria);
    }
}