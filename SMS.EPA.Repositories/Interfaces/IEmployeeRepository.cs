namespace SMS.EPA.Repositories
{
    using Core.Model;
    using Fanex.Data.Repository;
    using Model.Enitites;
    using System.Collections.Generic;

    public interface IEmployeeRepository
    {
        IEnumerable<Employee> GetAllEmployee(ICriteria criteria);

        UserSession GetEmployeeLogin(ICriteria criteria);

        Employee GetEmployeeById(ICriteria criteria);
    }
}