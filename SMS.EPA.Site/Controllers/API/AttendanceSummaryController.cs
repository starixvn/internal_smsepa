// ----------------------------------------------------------------------
// <copyright file="AttendanceSummaryController.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/08/2015">Created.</change>
// </history>

namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
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
            bool showAll = false,
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
                ShowAll = showAll,
                ApproverId = CurrentUser.EmployeeInfo.EmployeeId
            };
            IEnumerable<AttendanceSummaryModel> attendanceEmployees = _attendanceSummaryService.GetAttendanceSummary(criteria);
            SessionManager.Set<IEnumerable<AttendanceSummaryModel>>(Constants.DisciplineSummaryReportSessionKey + CurrentUser.EmployeeInfo.EmployeeId, attendanceEmployees);
            return attendanceEmployees;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Read)]
        public IEnumerable<OtherViolationModel> GetOtherViolations(
            string employeeIds,
            DateTime startDate,
            DateTime endDate,
            DateTime? workDate = null)
        {
            var criteria = new GetOtherViolationsOfEmployeeCriteria
            {
                EmployeeIds = employeeIds,
                WorkDate = workDate,
                StartDate = startDate,
                EndDate = endDate
            };

            IEnumerable<OtherViolationModel> violations = _attendanceSummaryService.GetOtherViolations(criteria);
            return violations;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Host)]
        public CommandResult InsertOtherViolation(InsertOtherViolationsCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _attendanceSummaryService.InsertOtherViolation(criteria);
            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Host)]
        public CommandResult DeleteOtherViolation(
            int employeeId,
            DateTime workDate,
            string name)
        {
            var criteria = new DeleteOtherViolationCriteria
            {
                EmployeeId = employeeId,
                WorkDate = workDate,
                Name = name
            };

            CommandResult commandResult = _attendanceSummaryService.DeleteOtherViolation(criteria);
            return commandResult;
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
            bool showAll,
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
                HasIgnored = hasIgnored,
                ShowAll = showAll
            };

            IEnumerable<AttendanceDetailModel> attendanceViolations = _attendanceSummaryService.GetAttendanceDetail(criteria);
            SessionManager.Set<IEnumerable<AttendanceDetailModel>>(Constants.DisciplineDetailReportSessionKey + CurrentUser.EmployeeInfo.EmployeeId, attendanceViolations);
            return attendanceViolations;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.DisciplineSummary, Policy = Policy.Host)]
        public CommandResult UpdateIgnoredViolation(IEnumerable<IgnoredViolationModel> violations)
        {
            var criteria = new UpdateIgnoredViolationCriteria
            {
                ModifiedBy = CurrentUser.EmployeeInfo.EmployeeId
            };

            criteria.IgnoredViolations = criteria.ConvertViolationItemToXml(violations);
            CommandResult commandResult = _attendanceSummaryService.UpdateIgnoredViolation(criteria);
            return commandResult;
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

        [HttpGet]
        public bool CheckUserPermissionIsAdmin()
        {
            return CurrentUser != null ? CurrentUser.Roles.Any(x => x.RoleId == (int)SystemRole.SystemAdmin) : false;
        }
    }
}