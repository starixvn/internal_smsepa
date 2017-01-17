namespace SMS.EPA.ClientHrm.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Logging;
    using SMS.EPA.ClientHrm.Model;

    public class SynchronizeService : ISynchronizeService
    {
        private static TokenAccess _token;

        private readonly ITokenService tokenService;
        private readonly IDepartmentService departmentService;
        private readonly IEmployeeService employeeService;
        private readonly ITeamService teamService;
        private readonly IJobTitleService jobTitleService;
        private readonly ISupervisorService supervisorService;
        private readonly IDisciplineService disciplineService;
        private readonly IDisciplineSummaryService disciplineSummaryService;
        private readonly IPhotoService photoService;

        public SynchronizeService(
            ITokenService tokenService,
            IDepartmentService departmentService,
            IEmployeeService employeeService,
            ITeamService teamService,
            IJobTitleService jobtitleService,
            ISupervisorService supervisorService,
            IDisciplineService disciplineService,
            IDisciplineSummaryService disciplineSummaryService,
            IPhotoService photoService)
        {
            _token = new TokenAccess();
            this.tokenService = tokenService;
            this.departmentService = departmentService;
            this.employeeService = employeeService;
            this.teamService = teamService;
            this.jobTitleService = jobtitleService;
            this.supervisorService = supervisorService;
            this.disciplineService = disciplineService;
            this.disciplineSummaryService = disciplineSummaryService;
            this.photoService = photoService;
        }

        public void Synchronize()
        {
            SynchronizeDepartment();
            SynchronizeTeam();
            SynchronizeEmployee();
            SynchronizeJobTitle();
            SynchronizeEmployeeDisciplinePermission();
            SynchronizeGroupDisciplinePermission();
            SynchronizeDiscipline();
            SynchronizeDisciplineSummary();
            SynchronizePhoto();
        }

        private void SynchronizeDepartment()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime fromDate = departmentService.GetLastModifiedDate(_token);
                IEnumerable<Department> departments = departmentService.GetDepartments(fromDate);
                if (departments.Any())
                {
                    departmentService.Update(departments, _token);
                }
            }
        }

        private void SynchronizeTeam()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime fromDate = teamService.GetLastModifiedDate(_token);
                IEnumerable<Team> teams = teamService.GetTeams(fromDate);
                if (teams.Any())
                {
                    teamService.Update(teams, _token);
                }
            }
        }

        private void SynchronizeEmployee()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime fromDate = employeeService.GetLastModifiedDate(_token);
                IEnumerable<Employee> employees = employeeService.GetEmployees(fromDate);
                if (employees.Any())
                {
                    employeeService.Update(employees, _token);
                }
            }
        }

        private void SynchronizeJobTitle()
        {
            InitToken();

            if (_token.IsValid())
            {
                IEnumerable<JobTitle> jobTitles = jobTitleService.GetJobTitles();
                if (jobTitles.Any())
                {
                    jobTitleService.Update(jobTitles, _token);
                }
            }
        }

        private void SynchronizeEmployeeDisciplinePermission()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime fromDate = supervisorService.GetLastModifiedDateForEmployee(_token);
                IEnumerable<EmployeeDisciplinePermission> employees = supervisorService.GetEmployees(fromDate);
                if (employees.Any())
                {
                    supervisorService.UpdateEmployees(employees, _token);
                }
            }
        }

        private void SynchronizeGroupDisciplinePermission()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime fromDate = supervisorService.GetLastModifiedDateForGroup(_token);
                IEnumerable<GroupDisciplinePermission> groups = supervisorService.GetGroups(fromDate);
                if (groups.Any())
                {
                    supervisorService.UpdateGroups(groups, _token);
                }
            }
        }

        private void SynchronizeDiscipline()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime fromDate = disciplineService.GetFromDateDiscipline(_token);
                DateTime lastModifiedDate = disciplineService.GetLastModifiedDate(_token);
                IEnumerable<Discipline> violations = disciplineService.GetDiscipline(fromDate, lastModifiedDate);
                if (violations.Any())
                {
                    disciplineService.Update(violations, _token);
                }
            }
        }

        private void SynchronizeDisciplineSummary()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime lastModifiedDate = disciplineSummaryService.GetLastModifiedDate(_token);
                IEnumerable<DisciplineSummary> violations = disciplineSummaryService.GetDisciplineSummary(lastModifiedDate);
                if (violations.Any())
                {
                    disciplineSummaryService.Update(violations, _token);
                }
            }
        }

        private void SynchronizePhoto()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime lastModifiedDate = photoService.GetLastModifiedDate(_token);
                IEnumerable<Photo> photos = photoService.GetPhotos(lastModifiedDate);
                if (photos.Any())
                {
                    photoService.Update(photos, _token);
                }
            }
        }

        private void InitToken()
        {
            if (!_token.IsValid())
            {
                _token = tokenService.Request(ApplicationConfig.ApplicationInfo);
            }
        }
    }
}