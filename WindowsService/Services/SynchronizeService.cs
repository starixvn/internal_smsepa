namespace SMS.EPA.WindowsService.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EPA.Model.API;
    using Model;

    public class SynchronizeService : ISynchronizeService
    {
        private readonly ITokenService tokenService;
        private readonly IDepartmentService departmentService;
        private readonly IEmployeeService employeeService;
        private readonly ITeamService teamService;
        private readonly IJobTitleService jobTitleService;
        private readonly ISupervisorService supervisorService;
        private readonly IDisciplineSummaryService disciplineSummaryService;
        private readonly IPhotoService photoService;
        private readonly IJobLevelService jobLevelService;
        private TokenAccess token = new TokenAccess();

        public SynchronizeService(
            ITokenService tokenService,
            IDepartmentService departmentService,
            IEmployeeService employeeService,
            ITeamService teamService,
            IJobTitleService jobtitleService,
            ISupervisorService supervisorService,
            IDisciplineSummaryService disciplineSummaryService,
            IPhotoService photoService,
            IJobLevelService jobLevelService)
        {
            this.tokenService = tokenService;
            this.departmentService = departmentService;
            this.employeeService = employeeService;
            this.teamService = teamService;
            this.jobTitleService = jobtitleService;
            this.supervisorService = supervisorService;
            this.disciplineSummaryService = disciplineSummaryService;
            this.photoService = photoService;
            this.jobLevelService = jobLevelService;
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
        }

        private async Task SynchronizeDepartmentAsync()
        {
            await InitToken();
            if (token.IsValid())
            {
                DateTime fromDate = await departmentService.GetLastModifiedDateAsync(token);
                IEnumerable<Department> departments = departmentService.GetDepartments(fromDate);
                if (departments.Any())
                {
                    await departmentService.Update(departments, token);
                }

                Console.WriteLine("Department");
            }
        }

        private async Task SynchronizeTeamAsync()
        {
            await InitToken();
            if (token.IsValid())
            {
                DateTime fromDate = await teamService.GetLastModifiedDateAsync(token);
                IEnumerable<Team> teams = teamService.GetTeams(fromDate);
                if (teams.Any())
                {
                    await teamService.UpdateAsync(teams, token);
                }

                Console.WriteLine("Team");
            }
        }

        private async Task SynchronizeEmployeeAsync()
        {
            await InitToken();
            if (token.IsValid())
            {
                DateTime fromDate = await employeeService.GetLastModifiedDateAsync(token);
                IEnumerable<Employee> employees = employeeService.GetEmployees(fromDate);
                if (employees.Any())
                {
                    await employeeService.UpdateAsync(employees, token);
                }

                Console.WriteLine("Employee");
            }
        }

        private async Task SynchronizeJobTitle()
        {
            await InitToken();
            if (token.IsValid())
            {
                IEnumerable<JobTitle> jobTitles = jobTitleService.GetJobTitles();
                if (jobTitles.Any())
                {
                    await jobTitleService.UpdateAsync(jobTitles, token);
                }

                Console.WriteLine("JobTitle");
            }
        }

        private async Task SynchronizeEmployeeDisciplinePermissionAsync()
        {
            await InitToken();
            if (token.IsValid())
            {
                DateTime fromDate = await supervisorService.GetLastModifiedDateForEmployeeAsync(token);
                IEnumerable<EmployeeDisciplinePermission> employees = supervisorService.GetEmployees(fromDate);
                if (employees.Any())
                {
                    await supervisorService.UpdateEmployeesAsync(employees, token);
                }

                Console.WriteLine("EmployeeDisciplinePermission");
            }
        }

        private async Task SynchronizeGroupDisciplinePermission()
        {
            await InitToken();
            if (token.IsValid())
            {
                DateTime fromDate = await supervisorService.GetLastModifiedDateForGroup(token);
                IEnumerable<TeamDisciplinePermission> groups = supervisorService.GetGroups(fromDate);
                if (groups.Any())
                {
                    await supervisorService.UpdateGroupsAsync(groups, token);
                }

                Console.WriteLine("GroupDisciplinePermission");
            }
        }

        private async Task SynchronizeDisciplineSummaryAsync()
        {
            await InitToken();
            if (token.IsValid())
            {
                DateTime lastModifiedDate = await disciplineSummaryService.GetLastModifiedDateAsync(token);
                IEnumerable<DisciplineSummary> violations = disciplineSummaryService.GetDisciplineSummary(lastModifiedDate);
                if (violations.Any())
                {
                    await disciplineSummaryService.UpdateAsync(violations, token);
                }

                Console.WriteLine("DisciplineSummary");
            }
        }

        private async Task SynchronizePhotoAsync()
        {
            await InitToken();
            if (token.IsValid())
            {
                DateTime lastModifiedDate = await photoService.GetLastModifiedDate(token);
                IEnumerable<Photo> photos = photoService.GetPhotos(lastModifiedDate);
                if (photos.Any())
                {
                    await photoService.UpdateAsync(photos, token);
                }

                Console.WriteLine("Photo");
            }
        }

        private async Task SynchronizeJobLevelAsync()
        {
            await InitToken();

            if (token.IsValid())
            {
                DateTime lastModifiedDate = await jobLevelService.GetLastModifiedDateAsync(token);
                IEnumerable<JobLevel> jobLevel = jobLevelService.GetJobLevel(lastModifiedDate);
                if (jobLevel.Any())
                {
                    await jobLevelService.Update(jobLevel, token);
                }
                Console.WriteLine("JobLevel");
            }
        }

        private async Task InitToken()
        {
            if (!token.IsValid())
            {
                token = await tokenService.Request(ApplicationConfig.ApplicationInfo);
            }
        }
    }
}