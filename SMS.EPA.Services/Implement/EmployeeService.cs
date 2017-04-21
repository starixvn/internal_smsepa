namespace SMS.EPA.Services.Implement
{
    using Core.Enumerations;
    using Core.Model;
    using Core.Security;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model.Enitites;
    using Repositories;
    using Resources;
    using System.Collections.Generic;
    using System.Linq;

    public class EmployeeService : BaseService, IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeService()
            : this(null)
        {
        }

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository ?? new EmployeeRepository();
        }

        public UserSession GetEmployeeLogin(GetEmployeeLoginCriteria criteria)
        {
            var employee = new UserSession();

            if (criteria != null && criteria.IsValid())
            {
                employee = _employeeRepository.GetEmployeeLogin(criteria);
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
                employees = _employeeRepository.GetAllEmployee(criteria);
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
                employee = _employeeRepository.GetEmployeeById(criteria);
            }

            return employee;
        }

        public CommandResult CreateSpecialEmployee(CreateSpecialEmployeeCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                criteria.Password = Encryption.ComputeHash(HashProvider.Sha1, criteria.Password);
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateSpecialEmployee(UpdateSpecialEmployeeCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
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
                    commandResult = Repository.Update(criteria);
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
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public IEnumerable<Employee> GetEmployeeByDepartmentId(int? departmentId)
        {
            var criteria = new GetEmployeeByDepartmentIdCriteria
            {
                DepartmentId = departmentId
            };
            IEnumerable<Employee> employees = new List<Employee>();

            if (criteria.IsValid())
            {
                employees = GetData<Employee>(criteria).OrderBy(i => i.NickName);
            }

            return employees;
        }

        public IEnumerable<Employee> GetEmployeeByGroupId(int groupId)
        {
            var criteria = new GetEmployeeByGroupIdCriteria
            {
                GroupId = groupId
            };
            IEnumerable<Employee> employees = new List<Employee>();

            if (criteria.IsValid())
            {
                employees = GetData<Employee>(criteria);
            }

            return employees;
        }

        public CommandResult InsertEmployeeIPAddress(InsertEmployeeIPAddressCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateHasMonthlyEPA(UpdateHasMonthlyEPACriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
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
                password = GetDataEntity<string>(criteria);
            }

            return password;
        }
    }
}