namespace SMS.EPA.ClientHrm.Services
{
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class SynchronizeService : ISynchronizeService
    {
        private static TokenAccess _token;

        private readonly ITokenService _tokenService;
        private readonly IDepartmentService _departmentService;
        private readonly IEmployeeService _employeeService;
        private readonly ITeamService _teamService;
        private readonly IJobTitleService _jobTitleService;
        private readonly ISupervisorService _supervisorService;
        private readonly IDisciplineService _disciplineService;
        private readonly IDisciplineSummaryService _disciplineSummaryService;
        private readonly IPhotoService _photoService;

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
            _tokenService = tokenService;
            _departmentService = departmentService;
            _employeeService = employeeService;
            _teamService = teamService;
            _jobTitleService = jobtitleService;
            _supervisorService = supervisorService;
            _disciplineService = disciplineService;
            _disciplineSummaryService = disciplineSummaryService;
            _photoService = photoService;
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
                DateTime fromDate = _departmentService.GetLastModifiedDate(_token);
                IEnumerable<Department> departments = _departmentService.GetDepartments(fromDate);
                if (departments.Any())
                {
                    _departmentService.Update(departments, _token);
                }
            }
        }

        private void SynchronizeTeam()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime fromDate = _teamService.GetLastModifiedDate(_token);
                IEnumerable<Team> teams = _teamService.GetTeams(fromDate);
                if (teams.Any())
                {
                    _teamService.Update(teams, _token);
                }
            }
        }

        private void SynchronizeEmployee()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime fromDate = _employeeService.GetLastModifiedDate(_token);
                IEnumerable<Employee> employees = _employeeService.GetEmployees(fromDate);
                if (employees.Any())
                {
                    _employeeService.Update(employees, _token);
                }
            }
        }

        private void SynchronizeJobTitle()
        {
            InitToken();

            if (_token.IsValid())
            {
                IEnumerable<JobTitle> jobTitles = _jobTitleService.GetJobTitles();
                if (jobTitles.Any())
                {
                    _jobTitleService.Update(jobTitles, _token);
                }
            }
        }

        private void SynchronizeEmployeeDisciplinePermission()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime fromDate = _supervisorService.GetLastModifiedDateForEmployee(_token);
                IEnumerable<EmployeeDisciplinePermission> employees = _supervisorService.GetEmployees(fromDate);
                if (employees.Any())
                {
                    _supervisorService.UpdateEmployees(employees, _token);
                }
            }
        }

        private void SynchronizeGroupDisciplinePermission()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime fromDate = _supervisorService.GetLastModifiedDateForGroup(_token);
                IEnumerable<GroupDisciplinePermission> groups = _supervisorService.GetGroups(fromDate);
                if (groups.Any())
                {
                    _supervisorService.UpdateGroups(groups, _token);
                }
            }
        }

        private void SynchronizeDiscipline()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime fromDate = _disciplineService.GetFromDateDiscipline(_token);
                DateTime lastModifiedDate = _disciplineService.GetLastModifiedDate(_token);
                IEnumerable<Discipline> violations = _disciplineService.GetDiscipline(fromDate, lastModifiedDate);
                if (violations.Any())
                {
                    _disciplineService.Update(violations, _token);
                }
            }
        }

        private void SynchronizeDisciplineSummary()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime lastModifiedDate = _disciplineSummaryService.GetLastModifiedDate(_token);
                IEnumerable<DisciplineSummary> violations = _disciplineSummaryService.GetDisciplineSummary(lastModifiedDate);
                if (violations.Any())
                {
                    _disciplineSummaryService.Update(violations, _token);
                }
            }
        }

        private void SynchronizePhoto()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime lastModifiedDate = _photoService.GetLastModifiedDate(_token);
                IEnumerable<Photo> photos = _photoService.GetPhotos(lastModifiedDate);
                if (photos.Any())
                {
                    _photoService.Update(photos, _token);
                }
            }
        }

        private void InitToken()
        {
            if (!_token.IsValid())
            {
                _token = _tokenService.Request(ApplicationConfig.ApplicationInfo);
            }
        }
    }
}