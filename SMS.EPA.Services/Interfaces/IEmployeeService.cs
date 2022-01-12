namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Model.Enitites;

    public interface IEmployeeService
    {
        bool ValidatePassword(string enterPassword, string employeePassword);

        IList<Employee> GetEmployee(GetEmployeeCriteria criteria);

        Employee GetEmployeeById(int employeeId);

        CommandResult CreateSpecialEmployee(CreateSpecialEmployeeCriteria criteria);

        CommandResult ChangePassword(ChangePasswordCriteria criteria);

        CommandResult ResetPassword(ResetPasswordCriteria criteria);

        string GetPasswordByEmployeeId(int employeeId);
    }
}