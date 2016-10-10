namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.Enitites;

    public interface IEmployeeRepository
    {
        IEnumerable<Employee> GetAllEmployee(ICriteria criteria);

        UserSession GetEmployeeLogin(ICriteria criteria);

        Employee GetEmployeeById(ICriteria criteria);
    }
}