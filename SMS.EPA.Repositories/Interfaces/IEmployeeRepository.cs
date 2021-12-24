namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Model.Enitites;

    public interface IEmployeeRepository
    {
        IEnumerable<Employee> GetAllEmployee(ICriteria criteria);

        Employee GetEmployeeById(ICriteria criteria);
    }
}