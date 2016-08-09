namespace SMS.EPA.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.Employee;

    public interface IEmployeeRepository
    {
        RawEmployeeModel GetAllEmployee(ICriteria criteria);

        UserSession GetEmployeeLogin(ICriteria criteria);

        EmployeeModel GetEmployeeById(ICriteria criteria);
    }
}