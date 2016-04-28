namespace SMS.EPA.ClientHRM.Services
{
    using Fanex.Data.Repository;
    using SMS.EPA.ClientHRM.Criteria;
    using SMS.EPA.ClientHRM.Model;
    using SMS.EPA.ClientHRM.ThirdPartyAPIs;
    using System;
    using System.Collections.Generic;

    public class SupervisorService : ISupervisorService
    {
        private readonly IDynamicRepository _dynamicRepository;
        private readonly ISupervisorAPI _supervisorAPI;

        public SupervisorService() : this(null, null)
        { 
        }

        public SupervisorService(IDynamicRepository dynamicRepository, ISupervisorAPI supervisorAPI)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
            _supervisorAPI = supervisorAPI ?? new SupervisorAPI();
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
            return _supervisorAPI.UpdateEmployees(employees, token);
        }

        public CommandResult UpdateGroups(IEnumerable<GroupSupervisor> groups, TokenAccess token)
        {
            return _supervisorAPI.UpdateGroups(groups, token);
        }

        public DateTime GetMaxModifiedDateForEmployee(TokenAccess token)
        {
            return _supervisorAPI.GetMaxModifiedDateForEmployee(token);
        }

        public DateTime GetMaxModifiedDateForGroup(TokenAccess token)
        {
            return _supervisorAPI.GetMaxModifiedDateForGroup(token);
        }
    }
}
