namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EPA.Model.API;
    using Model;
    using SMS.EPA.Core.Constants;

    public class SyncEPADataService : ISyncEPADataService
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(SyncEPADataService));
        private readonly ITokenService _tokenService;
        private readonly IDepartmentService _departmentService;
        private readonly IEmployeeService _employeeService;
        private readonly ITeamService _teamService;
        private readonly IJobTitleService _jobTitleService;
        private readonly ISupervisorService _supervisorService;
        private readonly IDisciplineSummaryService _disciplineSummaryService;
        private readonly IPhotoService _photoService;
        private readonly IJobLevelService jobLevelService;

        private TokenAccess _token = new TokenAccess();

#pragma warning disable S107 // Methods should not have too many parameters
        public SyncEPADataService(
            ITokenService tokenService,
            IDepartmentService departmentService,
            IEmployeeService employeeService,
            ITeamService teamService,
            IJobTitleService jobtitleService,
            ISupervisorService supervisorService,
            IDisciplineSummaryService disciplineSummaryService,
            IPhotoService photoService,
            IJobLevelService jobLevelService)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            _tokenService = tokenService;
            _departmentService = departmentService;
            _employeeService = employeeService;
            _teamService = teamService;
            _jobTitleService = jobtitleService;
            _supervisorService = supervisorService;
            _disciplineSummaryService = disciplineSummaryService;
            _photoService = photoService;
            this.jobLevelService = jobLevelService;
        }

        public async Task ExecuteAsync()
        {
            await SynchronizeDepartmentAsync();
            await SynchronizeTeamAsync();
            await SynchronizeEmployeeAsync();
            await SynchronizeJobTitle();
            await SynchronizeEmployeeDisciplinePermissionAsync();
            await SynchronizeGroupDisciplinePermission();
            await SynchronizeDisciplineSummaryAsync();
            await SynchronizePhotoAsync();
            await SynchronizeJobLevelAsync();
        }

        private async Task SynchronizeDepartmentAsync()
        {
            await InitToken();
            if (_token.IsValid())
            {
                DateTime fromDate = await _departmentService.GetLastModifiedDateAsync(_token);
                logger.Info(string.Format("sync department - last modified date: {0}", fromDate.ToString(Constants.ModifiedDateTimeFormat)));
                IEnumerable<Department> departments = _departmentService.GetDepartments(fromDate);
                logger.Info(string.Format("sync department - total: {0}", departments.Count()));
                if (departments.Any())
                {
                    await _departmentService.Update(departments, _token);
                }
            }
        }

        private async Task SynchronizeTeamAsync()
        {
            await InitToken();
            if (_token.IsValid())
            {
                DateTime fromDate = await _teamService.GetLastModifiedDateAsync(_token);
                logger.Info(string.Format("sync team - last modified date: {0}", fromDate.ToString(Constants.ModifiedDateTimeFormat)));
                IEnumerable<Team> teams = _teamService.GetTeams(fromDate);
                logger.Info(string.Format("sync team - total: {0}", teams.Count()));
                if (teams.Any())
                {
                    await _teamService.UpdateAsync(teams, _token);
                }
            }
        }

        private async Task SynchronizeEmployeeAsync()
        {
            await InitToken();
            if (_token.IsValid())
            {
                DateTime fromDate = await _employeeService.GetLastModifiedDateAsync(_token);
                logger.Info(string.Format("sync employee - last modified date: {0}", fromDate.ToString(Constants.ModifiedDateTimeFormat)));
                IEnumerable<Employee> employees = _employeeService.GetEmployees(fromDate);
                logger.Info(string.Format("sync employee - total: {0}", employees.Count()));
                if (employees.Any())
                {
                    await _employeeService.UpdateAsync(employees, _token);
                }
            }
        }

        private async Task SynchronizeJobTitle()
        {
            await InitToken();
            if (_token.IsValid())
            {
                IEnumerable<JobTitle> jobTitles = _jobTitleService.GetJobTitles();
                if (jobTitles.Any())
                {
                    await _jobTitleService.UpdateAsync(jobTitles, _token);
                }
            }
        }

        private async Task SynchronizeEmployeeDisciplinePermissionAsync()
        {
            await InitToken();
            if (_token.IsValid())
            {
                DateTime fromDate = await _supervisorService.GetLastModifiedDateForEmployeeAsync(_token);
                IEnumerable<EmployeeDisciplinePermission> employees = _supervisorService.GetEmployees(fromDate);
                if (employees.Any())
                {
                    await _supervisorService.UpdateEmployeesAsync(employees, _token);
                }
            }
        }

        private async Task SynchronizeGroupDisciplinePermission()
        {
            await InitToken();
            if (_token.IsValid())
            {
                DateTime fromDate = await _supervisorService.GetLastModifiedDateForGroup(_token);
                IEnumerable<TeamDisciplinePermission> groups = _supervisorService.GetGroups(fromDate);
                if (groups.Any())
                {
                    await _supervisorService.UpdateGroupsAsync(groups, _token);
                }
            }
        }

        private async Task SynchronizeDisciplineSummaryAsync()
        {
            await InitToken();
            if (_token.IsValid())
            {
                DateTime lastModifiedDate = await _disciplineSummaryService.GetLastModifiedDateAsync(_token);
                IEnumerable<DisciplineSummary> violations = _disciplineSummaryService.GetDisciplineSummary(lastModifiedDate);
                if (violations.Any())
                {
                    await _disciplineSummaryService.UpdateAsync(violations, _token);
                }
            }
        }

        private async Task SynchronizePhotoAsync()
        {
            await InitToken();
            if (_token.IsValid())
            {
                DateTime lastModifiedDate = await _photoService.GetLastModifiedDate(_token);
                logger.Info(string.Format("sync photo - last modified date: {0}", lastModifiedDate.ToString(Constants.ModifiedDateTimeFormat)));
                IEnumerable<Photo> photos = _photoService.GetPhotos(lastModifiedDate);
                logger.Info(string.Format("sync photo: {0}", string.Join(",", photos.Select(x => x.Code))));
                if (photos.Any())
                {
                    await _photoService.UpdateAsync(photos, _token);
                }
            }
        }

        private async Task SynchronizeJobLevelAsync()
        {
            await InitToken();
            if (_token.IsValid())
            {
                DateTime lastModifiedDate = await jobLevelService.GetLastModifiedDateAsync(_token);
                IEnumerable<JobLevel> jobLevel = jobLevelService.GetJobLevel(lastModifiedDate);
                if (jobLevel.Any())
                {
                    await jobLevelService.Update(jobLevel, _token);
                }
            }
        }

        private async Task InitToken()
        {
            if (!_token.IsValid())
            {
                _token = await _tokenService.Request(ApplicationConfig.ApplicationInfo);
            }
        }
    }
}