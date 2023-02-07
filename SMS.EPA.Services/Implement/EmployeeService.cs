namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using HRM.UserControl.Client.Core.Models.Role;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Security;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories;
    using SMS.EPA.Resources;

    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IBaseService baseService;

        public EmployeeService() : this(null, null)
        {
        }

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IBaseService baseService)
        {
            this.employeeRepository = employeeRepository ?? new EmployeeRepository();
            this.baseService = baseService ?? new BaseService();
        }

        public bool ValidatePassword(string enterPassword, string employeePassword)
        {
            return Encryption.CompareStringToHash(HashProvider.Sha1, enterPassword, employeePassword);
        }

        public IList<Employee> GetEmployee(GetEmployeeCriteria criteria, IList<Role> roles)
        {
            IList<Employee> employees = employeeRepository.GetAllEmployee(criteria).ToList();
            foreach (var employee in employees)
            {
                employee.Roles = roles.Where(x => employee.Roles.Any(r => r.RoleId == x.RoleId)).ToList();
            }

            return employees;
        }

        public Employee GetEmployeeById(int employeeId, IList<Role> roles)
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = employeeId
            };
            var employee = new Employee();

            if (criteria.IsValid())
            {
                employee = employeeRepository.GetEmployeeById(criteria);
                employee.Roles = roles.Where(x => employee.Roles.Any(r => r.RoleId == x.RoleId)).ToList();
            }

            return employee;
        }

        public CommandResult CreateSpecialEmployee(CreateSpecialEmployeeCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                criteria.Password = Encryption.ComputeHash(HashProvider.Sha1, criteria.Password);
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public CommandResult ChangePassword(ChangePasswordCriteria criteria)
        {
            var commandResult = new CommandResult();
            string oldPassword = GetPasswordByEmployeeId(criteria.EmployeeId);
            if (ValidatePassword(criteria.OldPassword, oldPassword))
            {
                criteria.NewPassword = Encryption.ComputeHash(HashProvider.Sha1, criteria.NewPassword);
                criteria.ConfirmPassword = criteria.NewPassword;
                commandResult = baseService.ExecuteNonQuery(criteria);
            }
            else
            {
                commandResult.Message = Lang.ValidateWrongPassword;
            }

            return commandResult;
        }

        public CommandResult ResetPassword(ResetPasswordCriteria criteria)
        {
            criteria.NewPassword = Encryption.ComputeHash(HashProvider.Sha1, criteria.NewPassword);
            return baseService.ExecuteNonQuery(criteria);
        }

        public string GetPasswordByEmployeeId(int employeeId)
        {
            var criteria = new GetPasswordByEmpIdCriteria
            {
                EmployeeId = employeeId
            };
            var password = string.Empty;

            if (criteria.IsValid())
            {
                password = baseService.GetDataEntity<string>(criteria);
            }

            return password;
        }
    }
}