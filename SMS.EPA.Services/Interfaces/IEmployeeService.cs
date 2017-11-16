namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Model.Enitites;

    public interface IEmployeeService
    {
        UserSession GetEmployeeLogin(GetEmployeeLoginCriteria criteria);

        bool ValidatePassword(string enterPassword, string employeePassword);

        IEnumerable<Employee> GetAllEmployee(GetAllEmployeeCriteria criteria);

        Employee GetEmployeeById(int employeeId);

        CommandResult CreateSpecialEmployee(CreateSpecialEmployeeCriteria criteria);

        CommandResult ChangePassword(ChangePasswordCriteria criteria);

        CommandResult ResetPassword(ResetPasswordCriteria criteria);

        string GetPasswordByEmployeeId(int employeeId);
    }
}