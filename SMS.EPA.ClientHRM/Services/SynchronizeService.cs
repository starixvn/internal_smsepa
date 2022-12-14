namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EPA.Model.API;
    using Model;

    public class SynchronizeService : ISynchronizeService
    {
        private readonly ITokenService _tokenService;
        private readonly IDepartmentService _departmentService;
        private readonly IEmployeeService _employeeService;
        private readonly ITeamService _teamService;
        private readonly IJobTitleService _jobTitleService;
        private readonly ISupervisorService _supervisorService;
        private readonly IDisciplineSummaryService _disciplineSummaryService;
        private readonly IPhotoService _photoService;
        private readonly IJobLevelService jobLevelService;
        private readonly IUserMappingService userMappingService;

        private TokenAccess _token = new TokenAccess();

#pragma warning disable S107 // Methods should not have too many parameters
        public SynchronizeService(
            ITokenService tokenService,
            IDepartmentService departmentService,
            IEmployeeService employeeService,
            ITeamService teamService,
            IJobTitleService jobtitleService,
            ISupervisorService supervisorService,
            IDisciplineSummaryService disciplineSummaryService,
            IPhotoService photoService,
            IJobLevelService jobLevelService,
            IUserMappingService userMappingService)
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
            this.userMappingService = userMappingService;
        }

        public async Task Synchronize()
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
            await SynchronizeUserMappingAsync();
        }

        private async Task SynchronizeDepartmentAsync()
        {
            await InitToken();
            if (_token.IsValid())
            {
                DateTime fromDate = await _departmentService.GetLastModifiedDateAsync(_token);
                IEnumerable<Department> departments = _departmentService.GetDepartments(fromDate);
                if (departments.Any())
                {
                    await _departmentService.Update(departments, _token);
                }

                Console.WriteLine("Dept");
            }
        }

        private async Task SynchronizeTeamAsync()
        {
            await InitToken();
            if (_token.IsValid())
            {
                DateTime fromDate = await _teamService.GetLastModifiedDateAsync(_token);
                IEnumerable<Team> teams = _teamService.GetTeams(fromDate);
                if (teams.Any())
                {
                    await _teamService.UpdateAsync(teams, _token);
                }

                Console.WriteLine("Team");
            }
        }

        private async Task SynchronizeEmployeeAsync()
        {
            await InitToken();
            if (_token.IsValid())
            {
                DateTime fromDate = await _employeeService.GetLastModifiedDateAsync(_token);
                IEnumerable<Employee> employees = _employeeService.GetEmployees(fromDate);
                if (employees.Any())
                {
                    await _employeeService.UpdateAsync(employees, _token);
                }

                Console.WriteLine("Employee");
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

                Console.WriteLine("JobTitle");
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

                Console.WriteLine("EmployeeDisciplinePermission");
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

                Console.WriteLine("GroupDisciplinePermission");
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

                Console.WriteLine("DisciplineSummary");
            }
        }

        private async Task SynchronizePhotoAsync()
        {
            await InitToken();
            if (_token.IsValid())
            {
                DateTime lastModifiedDate = await _photoService.GetLastModifiedDate(_token);
                IEnumerable<Photo> photos = _photoService.GetPhotos(lastModifiedDate);
                if (photos.Any())
                {
                    await _photoService.UpdateAsync(photos, _token);
                }

                Console.WriteLine("Photo");
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
                Console.WriteLine("JobLevel");
            }
        }

        private async Task SynchronizeUserMappingAsync()
        {
            var lastModifiedDate = await userMappingService.GetLastModifiedDateAsync();
            var userMappings = await userMappingService.GetUserMappingAsync(lastModifiedDate);

            await userMappingService.UpdateUserMappingAsync(userMappings);
            Console.WriteLine("User Mapping");
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