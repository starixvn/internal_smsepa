namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.Employee;

    public interface IEmployeeRepository
    {
        IEnumerable<EmployeeModel> GetAllEmployee(ICriteria criteria);

        UserSession GetEmployeeLogin(ICriteria criteria);

        EmployeeModel GetEmployeeById(ICriteria criteria);
    }
}