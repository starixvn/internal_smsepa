namespace SMS.EPA.ClientHrm.Services
{
    using Core.Model;
    using Criteria;
    using EPA.Model.API;
    using Fanex.Data.Repository;
    using Model;
    using System;
    using System.Collections.Generic;
    using ThirdPartyAPI;

    public class DisciplinePermissionService : ISupervisorService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly IEmployeeSupervisorAPI _employeeSupervisorApi;
        private readonly ITeamSupervisorAPI _teamSupervisorApi;

        public DisciplinePermissionService(
            IDynamicRepository dynamicRepository,
            IEmployeeSupervisorAPI employeeSupervisorApi,
            ITeamSupervisorAPI teamSupervisorApi)
        {
            _dynamicRepository = dynamicRepository;
            _employeeSupervisorApi = employeeSupervisorApi;
            _teamSupervisorApi = teamSupervisorApi;
        }

        public IEnumerable<EmployeeDisciplinePermission> GetEmployees(DateTime fromDate)
        {
            var criteria = new GetEmployeeDisciplinePermissionCriteria { FromDate = fromDate };
            IEnumerable<EmployeeDisciplinePermission> employeeSupervisors = _dynamicRepository.Fetch<EmployeeDisciplinePermission>(criteria);

            return employeeSupervisors;
        }

        public IEnumerable<TeamDisciplinePermission> GetGroups(DateTime fromDate)
        {
            var criteria = new GetGroupDisciplinePermissionCriteria { FromDate = fromDate };
            IEnumerable<TeamDisciplinePermission> groupSupervisors = _dynamicRepository.Fetch<TeamDisciplinePermission>(criteria);

            return groupSupervisors;
        }

        public CommandResult UpdateEmployees(IEnumerable<EmployeeDisciplinePermission> employees, TokenAccess token)
        {
            return _employeeSupervisorApi.Update(employees, token);
        }

        public CommandResult UpdateGroups(IEnumerable<TeamDisciplinePermission> groups, TokenAccess token)
        {
            return _teamSupervisorApi.Update(groups, token);
        }

        public DateTime GetLastModifiedDateForEmployee(TokenAccess token)
        {
            return _employeeSupervisorApi.GetLastModifiedDate(token);
        }

        public DateTime GetLastModifiedDateForGroup(TokenAccess token)
        {
            return _teamSupervisorApi.GetLastModifiedDate(token);
        }
    }
}