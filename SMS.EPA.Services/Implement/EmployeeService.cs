namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Security;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Interfaces;

    public class EmployeeService : BaseService, IEmployeeService
    {
        private readonly IEmployeeRepository employeeRepository;

        public EmployeeService()
            : this(null)
        {
        }

        public EmployeeService(IEmployeeRepository employeeRepository)
        {
            this.employeeRepository = employeeRepository ?? new EmployeeRepository();
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

        public bool CheckValidateEmployee(string employeePassword, string enterPassword)
        {
            bool isValid = !string.IsNullOrWhiteSpace(enterPassword) &&
                                Encryption.CompareStringToHash(HashProvider.Sha1, enterPassword, employeePassword);

            return isValid;
        }

        public IEnumerable<EmployeeModel> GetAllEmployee(GetAllEmployeeCriteria criteria)
        {
            IEnumerable<EmployeeModel> employees = new List<EmployeeModel>();

            if (criteria != null && criteria.IsValid())
            {
                employees = employeeRepository.GetAllEmployee(criteria);
            }

            return employees;
        }

        public EmployeeModel GetEmployeeById(int employeeId)
        {
            var criteria = new GetEmployeeByIdCriteria
            {
                EmployeeId = employeeId
            };
            var employee = new EmployeeModel();

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
                string oldPassword = this.GetPasswordByEmployeeId(criteria.EmployeeId);
                if (Encryption.CompareStringToHash(HashProvider.Sha1, criteria.OldPassword, oldPassword))
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

        public CommandResult AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public IEnumerable<EmployeeModel> GetEmployeeByDepartmentId(int? departmentId)
        {
            var criteria = new GetEmployeeByDepartmentIdCriteria
            {
                DepartmentId = departmentId
            };
            IEnumerable<EmployeeModel> employees = new List<EmployeeModel>();

            if (criteria.IsValid())
            {
                employees = GetData<EmployeeModel>(criteria).OrderBy(i => i.NickName);
            }

            return employees;
        }

        public IEnumerable<EmployeeModel> GetEmployeeByGroupId(int groupId)
        {
            var criteria = new GetEmployeeByGroupIdCriteria
            {
                GroupId = groupId
            };
            IEnumerable<EmployeeModel> employees = new List<EmployeeModel>();

            if (criteria.IsValid())
            {
                employees = GetData<EmployeeModel>(criteria);
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