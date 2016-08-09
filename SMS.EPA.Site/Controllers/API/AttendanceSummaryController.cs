namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Criteria.AttendanceSummary;
    using SMS.EPA.Model.AttendanceSummary;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class AttendanceSummaryController : BaseController
    {
        private readonly IAttendanceSummaryService attendanceSummaryService;

        public AttendanceSummaryController()
            : this(null)
        {
        }

        public AttendanceSummaryController(IAttendanceSummaryService attendanceSummaryService)
        {
            this.attendanceSummaryService = attendanceSummaryService ?? new AttendanceSummaryService();
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
                ApproverId = this.SessionService.EmployeeId
            };
            IEnumerable<AttendanceSummaryModel> attendanceEmployees = attendanceSummaryService.GetAttendanceSummary(criteria);

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

            IEnumerable<OtherViolationModel> violations = attendanceSummaryService.GetOtherViolations(criteria);

            return violations;
        }

        public IEnumerable<ViolationTypeModel> GetViolationType(
            bool includeOtherViolate,
            bool? isActive = null)
        {
            var criteria = new GetViolationTypeCriteria
            {
                IncludeOtherViolate = includeOtherViolate,
                IsActive = isActive
            };
            IEnumerable<ViolationTypeModel> violations = attendanceSummaryService.GetViolationType(criteria);

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
                ApproverId = this.SessionService.EmployeeId,
                HasIgnored = hasIgnored
            };

            IEnumerable<AttendanceDetailModel> attendanceViolations = attendanceSummaryService.GetAttendanceDetail(criteria);

            return attendanceViolations;
        }

        public int GetUserPermissionOnAttendanceSummary()
        {
            int permission = this.SessionService.GetPermission(EPAFunctionKey.DisciplineSummary);
            return permission;
        }
    }
}