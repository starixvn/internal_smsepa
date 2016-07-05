namespace SMS.EPA.ClientHRM.Services
{
    using Fanex.Data.Repository;
    using SMS.EPA.ClientHRM.Criteria;
    using SMS.EPA.ClientHRM.Model;
    using SMS.EPA.ClientHRM.ThirdPartyAPIs;
    using System;
    using System.Collections.Generic;

    public class DisciplinePermissionService : ISupervisorService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly IEmployeeSupervisorAPI _employeeSupervisorAPI;
        private readonly ITeamSupervisorAPI _teamSupervisorAPI;

        public DisciplinePermissionService(
            IDynamicRepository dynamicRepository, 
            IEmployeeSupervisorAPI employeeSupervisorAPI,
            ITeamSupervisorAPI teamSupervisorAPI)
        {
            _dynamicRepository = dynamicRepository;
            _employeeSupervisorAPI = employeeSupervisorAPI;
            _teamSupervisorAPI = teamSupervisorAPI;
        }

        public IEnumerable<EmployeeDisciplinePermission> GetEmployees(DateTime fromDate)
        {
            var criteria = new GetEmployeeDisciplinePermissionCriteria { FromDate = fromDate };
            IEnumerable<EmployeeDisciplinePermission> employeeSupervisors = _dynamicRepository.Fetch<EmployeeDisciplinePermission>(criteria);

            return employeeSupervisors;
        }

        public IEnumerable<GroupDisciplinePermission> GetGroups(DateTime fromDate)
        {
            var criteria = new GetGroupDisciplinePermissionCriteria { FromDate = fromDate };
            IEnumerable<GroupDisciplinePermission> groupSupervisors = _dynamicRepository.Fetch<GroupDisciplinePermission>(criteria);

            return groupSupervisors;
        }

        public CommandResult UpdateEmployees(IEnumerable<EmployeeDisciplinePermission> employees, TokenAccess token)
        {
            return _employeeSupervisorAPI.Update(employees, token);
        }

        public CommandResult UpdateGroups(IEnumerable<GroupDisciplinePermission> groups, TokenAccess token)
        {
            return _teamSupervisorAPI.Update(groups, token);
        }

        public DateTime GetMaxModifiedDateForEmployee(TokenAccess token)
        {
            return _employeeSupervisorAPI.GetMaxModifiedDate(token);
        }

        public DateTime GetMaxModifiedDateForGroup(TokenAccess token)
        {
            return _teamSupervisorAPI.GetMaxModifiedDate(token);
        }
    }
}
