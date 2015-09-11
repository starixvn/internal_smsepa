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
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria.AttendanceSummary;
    using SMS.EPA.Model.AttendanceSummary;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class AttendanceSummaryController : BaseController
    {
        private readonly IAttendanceSummaryService _attendanceSummaryService;

        public AttendanceSummaryController()
            : this(null)
        {
        }

        public AttendanceSummaryController(IAttendanceSummaryService attendanceSummaryService)
        {
            _attendanceSummaryService = attendanceSummaryService ?? new AttendanceSummaryService();
        }

        public IEnumerable<AttendanceSummaryModel> GetAttendanceSummary(
            DateTime startDate,
            DateTime endDate,
            int? departmentId = null,
            int? employeeId = null)
        {
            var criteria = new GetAttendanceSummaryCriteria
            {
                StartDate = startDate,
                EndDate = endDate,
                DepartmentId = departmentId,
                EmployeeId = employeeId
            };
            IEnumerable<AttendanceSummaryModel> attendanceEmployees = _attendanceSummaryService.GetAttendanceSummary(criteria);
            return attendanceEmployees;
        }

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

        public CommandResult InsertOtherViolation(InsertOtherViolationsCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = CurrentUser.EmployeeInfo.EmployeeId;
            }

            CommandResult commandResult = _attendanceSummaryService.InsertOtherViolation(criteria);
            return commandResult;
        }

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
    }
}
