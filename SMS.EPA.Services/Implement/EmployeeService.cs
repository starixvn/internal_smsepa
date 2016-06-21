namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Security;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Authentication;
    using SMS.EPA.Criteria.Employee;
    using SMS.EPA.Model.Authentication;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Model.RoleManagement;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

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

        public UserSession GetEmployeeByCode(GetEmployeeByCodeCriteria criteria)
        {
            UserSession employee = new UserSession();

            if (criteria != null && criteria.IsValid())
            {
                employee = _employeeRepository.GetEmployee(criteria);
            }

            return employee;
        }

        public UserSession GetEmployeeByUsername(GetEmployeeByDomainUsernameCriteria criteria)
        {
            UserSession employee = new UserSession();

            if (criteria != null && criteria.IsValid())
            {
                employee = _employeeRepository.GetEmployee(criteria);
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
                RawEmployeeModel rawEmployee = _employeeRepository.GetAllEmployee(criteria);
                employees = GetRoleOfEmployee(rawEmployee);
            }

            return employees;
        }

        public EmployeeModel GetEmployeeById(GetEmployeeByIdCriteria criteria)
        {
            var employee = new EmployeeModel();

            if (criteria != null && criteria.IsValid())
            {
                employee = _employeeRepository.GetEmployeeById(criteria);
            }

            return employee;
        }

        public IEnumerable<Group> GetAllGroup(GetAllGroupCriteria criteria)
        {
            IEnumerable<Group> groups = new List<Group>();

            if (criteria != null && criteria.IsValid())
            {
                groups = GetData<Group>(criteria);
            }

            return groups;
        }

        public CommandResult CreateSpecialEmployee(CreateSpecialEmployeeCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateSpecialEmployee(UpdateSpecialEmployeeCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public IEnumerable<JobTitle> GetAllJobTitle(GetAllJobTitleCriteria criteria)
        {
            IEnumerable<JobTitle> jobTitles = new List<JobTitle>();

            if (criteria != null && criteria.IsValid())
            {
                jobTitles = GetData<JobTitle>(criteria);
            }

            return jobTitles;
        }

        public CommandResult ChangePassword(ChangePasswordCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                criteria.NewPassword = Encryption.ComputeHash(HashProvider.Sha1, criteria.NewPassword);
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult AssignRoleToEmployee(AssignRoleToEmployeeCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public IEnumerable<EmployeeModel> GetEmployeeByDepartmentId(GetEmployeeByDepartmentIdCriteria criteria)
        {
            IEnumerable<EmployeeModel> employees = new List<EmployeeModel>();

            if (criteria != null && criteria.IsValid())
            {
                employees = GetData<EmployeeModel>(criteria).OrderBy(i => i.NickName);
            }

            return employees;
        }

        public IEnumerable<EmployeeModel> GetEmployeeByGroupId(GetEmployeeByGroupIdCriteria criteria)
        {
            IEnumerable<EmployeeModel> employees = new List<EmployeeModel>();

            if (criteria != null && criteria.IsValid())
            {
                employees = GetData<EmployeeModel>(criteria);
            }

            return employees;
        }

        public CommandResult InsertEmployeeIPAddress(InsertEmployeeIPAddressCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateHasMonthlyEPA(UpdateHasMonthlyEPACriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        private IEnumerable<EmployeeModel> GetRoleOfEmployee(RawEmployeeModel rawEmployee)
        {
            foreach (EmployeeModel employee in rawEmployee.Employees)
            {
                IList<SystemRoleModel> employeeRoles = rawEmployee.Roles.Where(role => role.EmployeeId == employee.EmployeeId).Select(role => new SystemRoleModel
                {
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    RoleCode = role.RoleCode
                }).ToList();

                employee.Roles = employeeRoles;
            }

            return rawEmployee.Employees;
        }
    }
}