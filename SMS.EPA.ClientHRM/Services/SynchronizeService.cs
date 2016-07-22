namespace SMS.EPA.ClientHRM.Services
{
    using Fanex.Logging;
    using SMS.EPA.ClientHRM.Model;
    using SMS.EPA.ServicesAPI.Model;
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
        private readonly IAttendanceService _attendanceService;

        public SynchronizeService(
            ITokenService tokenService,
            IDepartmentService departmentService,
            IEmployeeService employeeService,
            ITeamService teamService,
            IJobTitleService jobtitleService,
            ISupervisorService supervisorService,
            IAttendanceService attendanceService)
        {
            _token = new TokenAccess();
            _tokenService = tokenService;
            _departmentService = departmentService;
            _employeeService = employeeService;
            _teamService = teamService;
            _jobTitleService = jobtitleService;
            _supervisorService = supervisorService;
            _attendanceService = attendanceService;
        }

        public void Synchronize()
        {
            WriteLog("Department");
            SynchronizeDepartment();
            WriteLog("Team");
            SynchronizeTeam();
            WriteLog("Employee");
            SynchronizeEmployee();
            WriteLog("Jobtitle");
            SynchronizeJobTitle();
            WriteLog("Employee Discipline Permission");
            SynchronizeEmployeeDisciplinePermission();
            WriteLog("Section Discipline Permission");
            SynchronizeGroupDisciplinePermission();
            WriteLog("Discipline");
            SynchronizeAttendanceViolation();
            WriteLog("Done!");
        }

        private void WriteLog(string text)
        {
            if (ApplicationConfig.IsWriteLog)
            {
                LogService.Info(text);
            }
        }

        private void SynchronizeDepartment()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime fromDate = _departmentService.GetMaxModifiedDate(_token);
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
                DateTime fromDate = _teamService.GetMaxModifiedDate(_token);
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
                DateTime fromDate = _employeeService.GetMaxModifiedDate(_token);
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
                DateTime fromDate = _supervisorService.GetMaxModifiedDateForEmployee(_token);
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
                DateTime fromDate = _supervisorService.GetMaxModifiedDateForGroup(_token);
                IEnumerable<GroupDisciplinePermission> groups = _supervisorService.GetGroups(fromDate);
                if (groups.Any())
                {
                    _supervisorService.UpdateGroups(groups, _token);
                }
            }
        }

        private void SynchronizeAttendanceViolation()
        {
            InitToken();

            if (_token.IsValid())
            {
                DateTime fromDate = _attendanceService.GetFromDateAttendanceViolation(_token);
                DateTime lastModifiedDate = _attendanceService.GetMaxModifiedDate(_token);
                IEnumerable<AttendanceViolation> violations = _attendanceService.GetViolation(fromDate, lastModifiedDate);
                if (violations.Any())
                {
                    _attendanceService.Update(violations, _token);
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
