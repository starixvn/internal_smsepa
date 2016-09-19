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
        private readonly IDynamicRepository dynamicRepository;
        private readonly IEmployeeSupervisorAPI employeeSupervisorAPI;
        private readonly ITeamSupervisorAPI teamSupervisorAPI;

        public DisciplinePermissionService(
            IDynamicRepository dynamicRepository, 
            IEmployeeSupervisorAPI employeeSupervisorAPI,
            ITeamSupervisorAPI teamSupervisorAPI)
        {
            this.dynamicRepository = dynamicRepository;
            this.employeeSupervisorAPI = employeeSupervisorAPI;
            this.teamSupervisorAPI = teamSupervisorAPI;
        }

        public IEnumerable<EmployeeDisciplinePermission> GetEmployees(DateTime fromDate)
        {
            var criteria = new GetEmployeeDisciplinePermissionCriteria { FromDate = fromDate };
            IEnumerable<EmployeeDisciplinePermission> employeeSupervisors = dynamicRepository.Fetch<EmployeeDisciplinePermission>(criteria);

            return employeeSupervisors;
        }

        public IEnumerable<GroupDisciplinePermission> GetGroups(DateTime fromDate)
        {
            var criteria = new GetGroupDisciplinePermissionCriteria { FromDate = fromDate };
            IEnumerable<GroupDisciplinePermission> groupSupervisors = dynamicRepository.Fetch<GroupDisciplinePermission>(criteria);

            return groupSupervisors;
        }

        public CommandResult UpdateEmployees(IEnumerable<EmployeeDisciplinePermission> employees, TokenAccess token)
        {
            return employeeSupervisorAPI.Update(employees, token);
        }

        public CommandResult UpdateGroups(IEnumerable<GroupDisciplinePermission> groups, TokenAccess token)
        {
            return teamSupervisorAPI.Update(groups, token);
        }

        public DateTime GetLastModifiedDateForEmployee(TokenAccess token)
        {
            return employeeSupervisorAPI.GetLastModifiedDate(token);
        }

        public DateTime GetLastModifiedDateForGroup(TokenAccess token)
        {
            return teamSupervisorAPI.GetLastModifiedDate(token);
        }
    }
}
