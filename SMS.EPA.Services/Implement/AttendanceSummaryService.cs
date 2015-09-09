// ----------------------------------------------------------------------
// <copyright file="AttendanceSummaryService.cs" company="Nexcel Solutions Viet Nam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// ------------------------------------------------------------------------
// <history>
//   <change who="Josh.Phan" date="9/08/2015">Created.</change>
// </history>

namespace SMS.EPA.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.AttendanceSummary;
    using SMS.EPA.Model.AttendanceSummary;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class AttendanceSummaryService : BaseService, IAttendanceSummaryService
    {
        private IAttendanceSummaryRepository _attendanceSummaryRepository;

        public AttendanceSummaryService()
            : this(null)
        {
        }

        public AttendanceSummaryService(IAttendanceSummaryRepository attendanceSummaryRepository)
        {
            _attendanceSummaryRepository = attendanceSummaryRepository ?? new AttendanceSummaryRepository();
        }

        public IEnumerable<AttendanceSummaryModel> GetAttendanceSummary(GetAttendanceSummaryCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<AttendanceSummaryModel> attendanceEmployees = new List<AttendanceSummaryModel>();
            var rawAttendanceSummary = new RawAttendanceSummaryModel();

            if (criteria.IsValid())
            {
                rawAttendanceSummary = _attendanceSummaryRepository.GetAttendanceSummary(criteria);
                attendanceEmployees = GroupEmployeeAndCalcualteScore(rawAttendanceSummary);
            }

            return attendanceEmployees;
        }

        private IEnumerable<AttendanceSummaryModel> GroupEmployeeAndCalcualteScore(RawAttendanceSummaryModel rawAttendanceSummary)
        {
            throw new NotImplementedException();
        }
    }
}
