namespace SMS.EPA.Repositories
{
    using Fanex.Data.Repository;
    using Model.Enitites;
    using System.Collections.Generic;

    public interface IDepartmentRepository
    {
        IEnumerable<Department> GetDepartment(ICriteria criteria);
    }
}