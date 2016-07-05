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

        public IEnumerable<EmployeeSupervisor> GetEmployees(DateTime fromDate)
        {
            var criteria = new GetEmployeeSupervisorCriteria { FromDate = fromDate };
            IEnumerable<EmployeeSupervisor> employeeSupervisors = _dynamicRepository.Fetch<EmployeeSupervisor>(criteria);

            return employeeSupervisors;
        }

        public IEnumerable<GroupSupervisor> GetGroups(DateTime fromDate)
        {
            var criteria = new GetGroupSupervisorCriteria { FromDate = fromDate };
            IEnumerable<GroupSupervisor> groupSupervisors = _dynamicRepository.Fetch<GroupSupervisor>(criteria);

            return groupSupervisors;
        }

        public CommandResult UpdateEmployees(IEnumerable<EmployeeSupervisor> employees, TokenAccess token)
        {
            return _employeeSupervisorAPI.Update(employees, token);
        }

        public CommandResult UpdateGroups(IEnumerable<GroupSupervisor> groups, TokenAccess token)
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
