namespace SMS.EPA.Services.Interfaces
{
    using Core.Model;
    using Criteria;
    using Model.Enitites;
    using System.Collections.Generic;

    public interface IEmployeeService
    {
        UserSession GetEmployeeLogin(GetEmployeeLoginCriteria criteria);

        bool ValidatePassword(string enterPassword, string employeePassword);

        IEnumerable<Employee> GetAllEmployee(GetAllEmployeeCriteria criteria);

        Employee GetEmployeeById(int employeeId);

        CommandResult CreateSpecialEmployee(CreateSpecialEmployeeCriteria criteria);

        CommandResult UpdateSpecialEmployee(UpdateSpecialEmployeeCriteria criteria);

        CommandResult ChangePassword(ChangePasswordCriteria criteria);

        CommandResult ResetPassword(ResetPasswordCriteria criteria);

        CommandResult AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria);

        IEnumerable<Employee> GetEmployeeByDepartmentId(int? departmentId);

        IEnumerable<Employee> GetEmployeeByGroupId(int groupId);

        CommandResult InsertEmployeeIPAddress(InsertEmployeeIPAddressCriteria criteria);

        CommandResult UpdateHasMonthlyEPA(UpdateHasMonthlyEPACriteria criteria);

        string GetPasswordByEmployeeId(int employeeId);
    }
}