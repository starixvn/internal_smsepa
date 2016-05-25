namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Criteria.AttendanceSummary;
    using SMS.EPA.Model.AttendanceSummary;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using SMS.EPA.Web;
    using SMS.EPA.Web.ApiAuthorizeAttribute;
    using SMS.EPA.Web.Services;

    public class AttendanceSummaryController : BaseController
    {
        private readonly IAttendanceSummaryService _attendanceSummaryService;
        private readonly IWebCommonService _webCommonService;

        public AttendanceSummaryController()
            : this(null, null)
        {
        }

        public AttendanceSummaryController(
            IAttendanceSummaryService attendanceSummaryService,
            IWebCommonService webCommonService)
        {
            _attendanceSummaryService = attendanceSummaryService ?? new AttendanceSummaryService();
            _webCommonService = webCommonService ?? new WebCommonService();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Read)]
        public IEnumerable<AttendanceSummaryModel> GetAttendanceSummary(
            DateTime startDate,
            DateTime endDate,
            int? departmentId = null,
            bool? isLocal = null,
            int? employeeId = null,
            int? reasonId = null)
        {
            var criteria = new GetAttendanceSummaryCriteria
            {
                StartDate = startDate,
                EndDate = endDate,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                EmployeeId = employeeId,
                ReasonId = reasonId,
                ApproverId = CurrentUser.EmployeeInfo.EmployeeId
            };
            IEnumerable<AttendanceSummaryModel> attendanceEmployees = _attendanceSummaryService.GetAttendanceSummary(criteria);
            SessionManager.Set(Constants.DisciplineSummaryReportSessionKey + CurrentUser.EmployeeInfo.EmployeeId, attendanceEmployees);

            return attendanceEmployees;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Read)]
        public IEnumerable<OtherViolationModel> GetOtherViolations(
            int employeeId,
            DateTime startDate,
            DateTime endDate,
            DateTime? workDate = null)
        {
            var criteria = new GetOtherViolationsOfEmployeeCriteria
            {
                EmployeeId = employeeId,
                WorkDate = workDate,
                StartDate = startDate,
                EndDate = endDate
            };

            IEnumerable<OtherViolationModel> violations = _attendanceSummaryService.GetOtherViolations(criteria);

            return violations;
        }

        public IEnumerable<ViolationModel> GetViolationType()
        {
            var criteria = new GetViolationTypeCriteria();
            IEnumerable<ViolationModel> violations = _attendanceSummaryService.GetViolationType(criteria);
            return violations;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Read)]
        public IEnumerable<AttendanceDetailModel> GetAttendanceDetail(
            DateTime startDate,
            DateTime endDate,
            int? employeeId = null,
            int? departmentId = null,
            bool? isLocal = null,
            int? reasonId = null,
            bool? hasIgnored = null)
        {
            var criteria = new GetDetailAttendanceCriteria
            {
                StartDate = startDate,
                EndDate = endDate,
                EmployeeId = employeeId,
                DepartmentId = departmentId,
                IsLocal = isLocal,
                ReasonId = reasonId,
                ApproverId = CurrentUser.EmployeeInfo.EmployeeId,
                HasIgnored = hasIgnored
            };

            IEnumerable<AttendanceDetailModel> attendanceViolations = _attendanceSummaryService.GetAttendanceDetail(criteria);
            SessionManager.Set(Constants.DisciplineDetailReportSessionKey + CurrentUser.EmployeeInfo.EmployeeId, attendanceViolations);

            return attendanceViolations;
        }

        [HttpGet]
        public IList<OtherViolationTypeModel> GetOtherViolationType(bool? isActive = null)
        {
            var criteria = new GetOtherViolationTypeCriteria
            {
                IsActive = isActive
            };

            IList<OtherViolationTypeModel> violationTypes = _attendanceSummaryService.GetOtherViolationType(criteria);
            return violationTypes;
        }

        public int GetUserPermissionOnAttendanceSummary()
        {
            int permission = _webCommonService.GetPermission(EPAFunctionKey.DisciplineSummary);
            return permission;
        }
    }
}