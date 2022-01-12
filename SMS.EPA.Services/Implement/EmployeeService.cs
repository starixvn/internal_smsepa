namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Enumerations;
    using Core.Model;
    using Core.Security;
    using Core.Services;
    using Criteria;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using Model.Enitites;
    using Repositories;
    using Resources;

    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IBaseService baseService;
        private readonly IUserControlSession userControlSession;
        private readonly IRoleService roleService;

        public EmployeeService() : this(null, null, null, null)
        {
        }

        public EmployeeService(
            IEmployeeRepository employeeRepository,
            IBaseService baseService,
            IUserControlSession userControlSession,
            IRoleService roleService)
        {
            this.employeeRepository = employeeRepository ?? new EmployeeRepository();
            this.baseService = baseService ?? new BaseService();
            this.userControlSession = userControlSession ?? new UserControlSession();
            this.roleService = roleService ?? new RoleService();
        }

        public bool ValidatePassword(string enterPassword, string employeePassword)
        {
            return Encryption.CompareStringToHash(HashProvider.Sha1, enterPassword, employeePassword);
        }

        public IList<Employee> GetEmployee(GetEmployeeCriteria criteria)
        {
            return employeeRepository.GetAllEmployee(criteria).ToList();
                var roles = roleService.GetBySiteId(userControlSession.SiteId);
                foreach(var employee in employees)
                {
                    employee.Roles = roles.Where(x => employee.Roles.Any(r => r.RoleId == x.RoleId)).ToList();
                }
        }

        public Employee GetEmployeeById(int employeeId)
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = employeeId
            };
            var employee = new Employee();

            if (criteria.IsValid())
            {
                var roles = roleService.GetBySiteId(userControlSession.SiteId);
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