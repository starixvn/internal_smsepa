namespace SMS.EPA.Repositories.Interfaces
{
    using Fanex.Data.Repository;
    using SMS.EPA.Model.Authentication;
    using SMS.EPA.Model.Employee;

    public interface IEmployeeRepository
    {
        RawEmployeeModel GetAllEmployee(ICriteria criteria);

        UserSession GetEmployee(ICriteria criteria);

        EmployeeModel GetEmployeeById(ICriteria criteria);
    }
}