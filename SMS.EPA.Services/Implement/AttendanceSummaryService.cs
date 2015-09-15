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
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.AttendanceSummary;
    using SMS.EPA.Model.AttendanceSummary;
    using SMS.EPA.Model.Enitites;
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

            if (criteria.IsValid())
            {
                attendanceEmployees = _attendanceSummaryRepository.GetAttendanceSummary(criteria);
            }

            return attendanceEmployees;
        }

        public IEnumerable<OtherViolationModel> GetOtherViolations(GetOtherViolationsOfEmployeeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<OtherViolationModel> violations = new List<OtherViolationModel>();

            if (criteria.IsValid())
            {
                violations = GetData<OtherViolationModel>(criteria);
            }

            return violations;
        }

        public CommandResult InsertOtherViolation(InsertOtherViolationsCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteOtherViolation(DeleteOtherViolationCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }

        public IEnumerable<ViolationModel> GetViolationType(GetViolationTypeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<ViolationModel> violations = new List<ViolationModel>();

            if (criteria.IsValid())
            {
                violations = GetData<ViolationModel>(criteria);
            }

            return violations;
        }

        public IEnumerable<AttendanceDetailModel> GetAttendanceDetail(GetDetailAttendanceCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            var attendanceViolations = new List<AttendanceDetailModel>();

            var firstAttendanceDetail = new AttendanceDetailModel
            {
                WorkDate = DateTime.Now,
                NickName = "Evan",
                Code="02358",
                Group = new Group
                {
                    GroupName = "UNIT 12 local"
                },
                Shift = "23>07",
                CheckIn = new ViolationModel
                {
                    HasViolated = false,
                    Value = "23:00"
                },
                CheckOut = new ViolationModel
                {
                    HasViolated = false,
                    Value = "04:55"
                },
                TotalHours = new ViolationModel
                {
                    HasViolated = false,
                    Value = "5:55"
                }
                ,
                LateIn = new ViolationModel
                {
                    HasViolated = false,
                    Value = string.Empty
                }
                ,
                EarlyOut = new ViolationModel
                {
                    HasViolated = true,
                    Value = "20"
                },
                Absent = new ViolationModel
                {
                    HasViolated = false,
                    Value = string.Empty
                },
                UnpaidLeave = new ViolationModel {
                    HasViolated = true,
                    Value = "1"
                },
                UnpaidLeaveAbsent = new ViolationModel
                {
                    HasViolated = false,
                    Value = string.Empty
                },
                ChangeShift = new ViolationModel
                {
                    HasViolated = false,
                    Value =  string.Empty
                },
                ApproverComment = "approved",
                EmployeeComment = "Early out",
                SystemSubtractScore = 5,
                ApproverSubtractScore = 5
            };

            attendanceViolations.Add(firstAttendanceDetail);
            return attendanceViolations;
        }
    }
}
