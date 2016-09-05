namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model.Employee;

    public interface IEmployeeService
    {
        UserSession GetEmployeeLogin(GetEmployeeLoginCriteria criteria);

        bool ValidatePassword(string enterPassword, string employeePassword);

        IEnumerable<EmployeeModel> GetAllEmployee(GetAllEmployeeCriteria criteria);

        EmployeeModel GetEmployeeById(int employeeId);

        CommandResult CreateSpecialEmployee(CreateSpecialEmployeeCriteria criteria);

        CommandResult UpdateSpecialEmployee(UpdateSpecialEmployeeCriteria criteria);

        CommandResult ChangePassword(ChangePasswordCriteria criteria);

        CommandResult AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria);

        IEnumerable<EmployeeModel> GetEmployeeByDepartmentId(int? departmentId);

        IEnumerable<EmployeeModel> GetEmployeeByGroupId(int groupId);

        CommandResult InsertEmployeeIPAddress(InsertEmployeeIPAddressCriteria criteria);

        CommandResult UpdateHasMonthlyEPA(UpdateHasMonthlyEPACriteria criteria);

        string GetPasswordByEmployeeId(int employeeId);
    }
}