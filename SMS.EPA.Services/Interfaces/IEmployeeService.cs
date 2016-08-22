namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.Authentication;
    using SMS.EPA.Model.Employee;

    public interface IEmployeeService
    {
        UserSession GetEmployeeLogin(GetEmployeeLoginCriteria criteria);

        bool CheckValidateEmployee(string employeePassword, string enterPassword);

        IEnumerable<EmployeeModel> GetAllEmployee(GetAllEmployeeCriteria criteria);

        EmployeeModel GetEmployeeById(GetEmployeeByIdCriteria criteria);

        CommandResult CreateSpecialEmployee(CreateSpecialEmployeeCriteria criteria);

        CommandResult UpdateSpecialEmployee(UpdateSpecialEmployeeCriteria criteria);

        CommandResult ChangePassword(ChangePasswordCriteria criteria);

        CommandResult AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria);

        IEnumerable<EmployeeModel> GetEmployeeByDepartmentId(GetEmployeeByDepartmentIdCriteria criteria);

        IEnumerable<EmployeeModel> GetEmployeeByGroupId(GetEmployeeByGroupIdCriteria criteria);

        CommandResult InsertEmployeeIPAddress(InsertEmployeeIPAddressCriteria criteria);

        CommandResult UpdateHasMonthlyEPA(UpdateHasMonthlyEPACriteria criteria);
    }
}