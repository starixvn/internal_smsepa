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
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Common.Helper;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.AttendanceSummary;
    using SMS.EPA.Model.AttendanceSummary;
    using SMS.EPA.Model.Employee;
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
            IEnumerable<AttendanceDetailModel> attendanceViolations = new List<AttendanceDetailModel>();

            if (criteria.IsValid())
            {
                RawAttendanceDetailModel rawAttendanceDetail = _attendanceSummaryRepository.GetAttendanceDetail(criteria);
                attendanceViolations = GroupAttendanceDetailAndCalculateScore(rawAttendanceDetail, attendanceViolations);
            }

            return attendanceViolations;
        }

        public CommandResult UpdateIgnoredViolation(UpdateIgnoredViolationCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            CommandResult commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        private static IEnumerable<AttendanceDetailModel> GroupAttendanceDetailAndCalculateScore(
            RawAttendanceDetailModel rawAttendanceDetail,
            IEnumerable<AttendanceDetailModel> attendanceViolations)
        {
            attendanceViolations = GroupAttendanceViolations(rawAttendanceDetail.Violations);
            attendanceViolations = GetCommentViolations(attendanceViolations, rawAttendanceDetail.CommentViolations);
            attendanceViolations = CalculateScore(attendanceViolations);
            attendanceViolations = GroupEmployeeInformation(attendanceViolations, rawAttendanceDetail.Employees);
            return attendanceViolations;
        }

        private static IEnumerable<AttendanceDetailModel> GroupAttendanceViolations(IEnumerable<AttendanceDetailModel> violations)
        {
            IEnumerable<AttendanceDetailModel> attendanceViolations = violations.GroupBy(x => new { x.EmployeeId, x.WorkDate, x.ShiftName }).Select(y => new AttendanceDetailModel
            {
                EmployeeId = y.Key.EmployeeId,
                WorkDate = y.Key.WorkDate,
                ShiftName = y.Key.ShiftName,
                CheckIn = GetCheckIn(y.ToList()),
                CheckOut = GetCheckOut(y.ToList()),
                TotalHours = GetTotalHours(y.ToList()),
                LateIn = GetLateIn(y.ToList()),
                EarlyOut = GetEarlyOut(y.ToList()),
                Absent = GetAbsent(y.ToList()),
                UnpaidLeaveAbsent = GetUnpaidLeaveAbsent(y.ToList()),
                UnpaidLeave = GetUnpaidLeave(y.ToList()),
                ChangeShift = GetChangeShift(y.ToList())
            }).ToList();

            return attendanceViolations;
        }

        private static IEnumerable<AttendanceDetailModel> GetCommentViolations(
            IEnumerable<AttendanceDetailModel> attendanceViolations,
            IEnumerable<AttendanceDetailModel> comments)
        {
            foreach (AttendanceDetailModel attendanceViolation in attendanceViolations)
            {
                IEnumerable<AttendanceDetailModel> commentItems =
                    comments.Where(x => x.WorkDate == attendanceViolation.WorkDate && x.EmployeeId == attendanceViolation.EmployeeId);

                if (commentItems != null && commentItems.Count() > 0)
                {
                    string approverComment = commentItems.First().ApproverComment;
                    string employeeComment = commentItems.First().EmployeeComment;

                    attendanceViolation.ApproverComment = GetRequestComment(approverComment);
                    attendanceViolation.EmployeeComment = GetRequestComment(employeeComment);
                }
            }

            return attendanceViolations;
        }

        private static IEnumerable<AttendanceDetailModel> CalculateScore(IEnumerable<AttendanceDetailModel> attendanceViolations)
        {
            foreach (AttendanceDetailModel attendanceDetail in attendanceViolations)
            {
                CalculateSubtractScore(attendanceDetail);
            }

            return attendanceViolations;
        }

        private static void CalculateSubtractScore(AttendanceDetailModel attendanceDetail)
        {
            if (attendanceDetail.CheckIn != null && attendanceDetail.CheckIn.HasViolated)
            {
                attendanceDetail.SystemSubtractScore += attendanceDetail.CheckIn.Score;
                attendanceDetail.ApproverSubtractScore += !attendanceDetail.CheckIn.HasIgnored ? attendanceDetail.CheckIn.Score : 0;
            }

            if (attendanceDetail.CheckOut != null && attendanceDetail.CheckOut.HasViolated)
            {
                attendanceDetail.SystemSubtractScore += attendanceDetail.CheckOut.Score;
                attendanceDetail.ApproverSubtractScore += !attendanceDetail.CheckOut.HasIgnored ? attendanceDetail.CheckOut.Score : 0;
            }

            if (attendanceDetail.LateIn != null && attendanceDetail.LateIn.HasViolated)
            {
                attendanceDetail.SystemSubtractScore += attendanceDetail.LateIn.Score;
                attendanceDetail.ApproverSubtractScore += !attendanceDetail.LateIn.HasIgnored ? attendanceDetail.LateIn.Score : 0;
            }

            if (attendanceDetail.EarlyOut != null && attendanceDetail.EarlyOut.HasViolated)
            {
                attendanceDetail.SystemSubtractScore += attendanceDetail.EarlyOut.Score;
                attendanceDetail.ApproverSubtractScore += !attendanceDetail.EarlyOut.HasIgnored ? attendanceDetail.EarlyOut.Score : 0;
            }

            if (attendanceDetail.TotalHours != null && attendanceDetail.TotalHours.HasViolated)
            {
                attendanceDetail.SystemSubtractScore += attendanceDetail.TotalHours.Score;
                attendanceDetail.ApproverSubtractScore += !attendanceDetail.TotalHours.HasIgnored ? attendanceDetail.TotalHours.Score : 0;
            }

            if (attendanceDetail.Absent != null && attendanceDetail.Absent.HasViolated)
            {
                attendanceDetail.SystemSubtractScore += attendanceDetail.Absent.Score;
                attendanceDetail.ApproverSubtractScore += !attendanceDetail.Absent.HasIgnored ? attendanceDetail.Absent.Score : 0;
            }

            if (attendanceDetail.UnpaidLeave != null && attendanceDetail.UnpaidLeave.HasViolated)
            {
                attendanceDetail.SystemSubtractScore += attendanceDetail.UnpaidLeave.Score;
                attendanceDetail.ApproverSubtractScore += !attendanceDetail.UnpaidLeave.HasIgnored ? attendanceDetail.UnpaidLeave.Score : 0;
            }

            if (attendanceDetail.UnpaidLeaveAbsent != null && attendanceDetail.UnpaidLeaveAbsent.HasViolated)
            {
                attendanceDetail.SystemSubtractScore += attendanceDetail.UnpaidLeaveAbsent.Score;
                attendanceDetail.ApproverSubtractScore += !attendanceDetail.UnpaidLeaveAbsent.HasIgnored ? attendanceDetail.UnpaidLeaveAbsent.Score : 0;
            }

            if (attendanceDetail.ChangeShift != null && attendanceDetail.ChangeShift.HasViolated)
            {
                attendanceDetail.SystemSubtractScore += attendanceDetail.ChangeShift.Score;
                attendanceDetail.ApproverSubtractScore += !attendanceDetail.ChangeShift.HasIgnored ? attendanceDetail.ChangeShift.Score : 0;
            }
        }

        private static IEnumerable<AttendanceDetailModel> GroupEmployeeInformation(
            IEnumerable<AttendanceDetailModel> attendanceViolations,
            IEnumerable<EmployeeModel> employees)
        {
            foreach (AttendanceDetailModel attendanceViolation in attendanceViolations)
            {
                attendanceViolation.Employee = employees.Where(x => x.EmployeeId == attendanceViolation.EmployeeId).First();
            }

            return attendanceViolations;
        }

        private static ViolationModel GetCheckIn(IEnumerable<AttendanceDetailModel> groupAttendances)
        {
            IEnumerable<AttendanceDetailModel> checkIns = groupAttendances.Where(x => x.CheckIn != null);
            var checkIn = new ViolationModel();

            if (checkIns != null && checkIns.Count() > 0)
            {
                checkIn = checkIns.First().CheckIn;
            }

            return checkIn;
        }

        private static ViolationModel GetCheckOut(List<AttendanceDetailModel> groupAttendances)
        {
            IEnumerable<AttendanceDetailModel> checkOuts = groupAttendances.Where(x => x.CheckOut != null);
            var checkOut = new ViolationModel();

            if (checkOuts != null && checkOuts.Count() > 0)
            {
                checkOut = checkOuts.First().CheckOut;
            }

            return checkOut;
        }

        private static ViolationModel GetTotalHours(List<AttendanceDetailModel> groupAttendances)
        {
            IEnumerable<AttendanceDetailModel> totalHours = groupAttendances.Where(x => x.TotalHours != null);
            var totalHour = new ViolationModel();

            if (totalHours != null && totalHours.Count() > 0)
            {
                totalHour = totalHours.First().TotalHours;
            }

            return totalHour;
        }

        private static ViolationModel GetLateIn(List<AttendanceDetailModel> groupAttendances)
        {
            IEnumerable<AttendanceDetailModel> lateIns = groupAttendances.Where(x => x.LateIn != null);
            var lateIn = new ViolationModel();

            if (lateIns != null && lateIns.Count() > 0)
            {
                lateIn = lateIns.First().LateIn;
            }

            return lateIn;
        }

        private static ViolationModel GetEarlyOut(List<AttendanceDetailModel> groupAttendances)
        {
            IEnumerable<AttendanceDetailModel> earlyOuts = groupAttendances.Where(x => x.EarlyOut != null);
            var earlyOut = new ViolationModel();

            if (earlyOuts != null && earlyOuts.Count() > 0)
            {
                earlyOut = earlyOuts.First().EarlyOut;
            }

            return earlyOut;
        }

        private static ViolationModel GetAbsent(List<AttendanceDetailModel> groupAttendances)
        {
            IEnumerable<AttendanceDetailModel> absents = groupAttendances.Where(x => x.Absent != null);
            var absent = new ViolationModel();

            if (absents != null && absents.Count() > 0)
            {
                absent = absents.First().Absent;
            }

            return absent;
        }

        private static ViolationModel GetUnpaidLeaveAbsent(List<AttendanceDetailModel> groupAttendances)
        {
            IEnumerable<AttendanceDetailModel> unpaidLeaveAbsents = groupAttendances.Where(x => x.UnpaidLeaveAbsent != null);
            var unpaidLeaveAbsent = new ViolationModel();

            if (unpaidLeaveAbsents != null && unpaidLeaveAbsents.Count() > 0)
            {
                unpaidLeaveAbsent = unpaidLeaveAbsents.First().UnpaidLeaveAbsent;
            }

            return unpaidLeaveAbsent;
        }

        private static ViolationModel GetUnpaidLeave(List<AttendanceDetailModel> groupAttendances)
        {
            IEnumerable<AttendanceDetailModel> unpaidLeaves = groupAttendances.Where(x => x.UnpaidLeave != null);
            var unpaidLeave = new ViolationModel();

            if (unpaidLeaves != null && unpaidLeaves.Count() > 0)
            {
                unpaidLeave = unpaidLeaves.First().UnpaidLeave;
            }

            return unpaidLeave;
        }

        private static ViolationModel GetChangeShift(List<AttendanceDetailModel> groupAttendances)
        {
            IEnumerable<AttendanceDetailModel> changeShifts = groupAttendances.Where(x => x.ChangeShift != null);
            var changeShift = new ViolationModel();

            if (changeShifts != null && changeShifts.Count() > 0)
            {
                changeShift = changeShifts.First().ChangeShift;
            }

            return changeShift;
        }

        private static string GetRequestComment(string comment)
        {
            string approverComment = string.Empty;

            if (!string.IsNullOrWhiteSpace(comment))
            {
                string[] comments = comment.Split(';');

                foreach (string commentItem in comments)
                {
                    string[] commentItems = commentItem.Split(':');

                    if (!string.IsNullOrWhiteSpace(commentItems[0]) && !string.IsNullOrWhiteSpace(commentItems[1]))
                    {
                        approverComment += commentItem;
                    }
                }
            }

            return approverComment;
        }
    }
}
