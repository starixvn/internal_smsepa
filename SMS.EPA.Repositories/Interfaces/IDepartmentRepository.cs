namespace SMS.EPA.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Enitites;
    using System.Collections.Generic;

    public interface IDepartmentRepository
    {
        IEnumerable<Department> GetDepartment(ICriteria criteria);
    }
}