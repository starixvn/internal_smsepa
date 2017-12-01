namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using Core.Enumerations;
    using Core.Model;
    using Core.Security;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model.Enitites;
    using Repositories;
    using Resources;

    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;
        private readonly IBaseService baseService;

        public EmployeeService() : this(null, null)
        {
        }

        public EmployeeService(IEmployeeRepository employeeRepository, IBaseService baseService)
        {
            this.employeeRepository = employeeRepository ?? new EmployeeRepository();
            this.baseService = baseService ?? new BaseService();
        }

        public UserSession GetEmployeeLogin(GetEmployeeLoginCriteria criteria)
        {
            var employee = new UserSession();

            if (criteria != null && criteria.IsValid())
            {
                employee = employeeRepository.GetEmployeeLogin(criteria);
            }

            return employee;
        }

        public bool ValidatePassword(string enterPassword, string employeePassword)
        {
            bool isValid = Encryption.CompareStringToHash(HashProvider.Sha1, enterPassword, employeePassword);
            return isValid;
        }

        public IEnumerable<Employee> GetAllEmployee(GetAllEmployeeCriteria criteria)
        {
            IEnumerable<Employee> employees = new List<Employee>();

            if (criteria != null && criteria.IsValid())
            {
                employees = employeeRepository.GetAllEmployee(criteria);
            }

            return employees;
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
                employee = employeeRepository.GetEmployeeById(criteria);
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

            if (criteria != null && criteria.IsValid())
            {
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
            }

            return commandResult;
        }

        public CommandResult ResetPassword(ResetPasswordCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                criteria.NewPassword = Encryption.ComputeHash(HashProvider.Sha1, criteria.NewPassword);
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return commandResult;
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