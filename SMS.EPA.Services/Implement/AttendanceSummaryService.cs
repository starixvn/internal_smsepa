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
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Fanex.Common.Helper;
    using NexFB.Core.Utilities;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.AttendanceSummary;
    using SMS.EPA.Model.AttendanceSummary;
    using SMS.EPA.Model.Employee;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Resources;
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
                attendanceEmployees = attendanceEmployees.OrderByDescending(x => x.Permission);
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
                attendanceViolations = attendanceViolations.OrderBy(x => x.Employee.Group.GroupId)
                    .ThenBy(x => x.Employee.Code)
                    .ThenByDescending(x => x.WorkDate);
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

        public IEnumerable<AttendanceScoreModel> GetAttendanceScoreByEPAId(GetAttendanceScoreByEPAIdCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IEnumerable<AttendanceScoreModel> attendanceScores = new List<AttendanceScoreModel>();

            if (criteria.IsValid())
            {
                attendanceScores = GetData<AttendanceScoreModel>(criteria);
            }

            return attendanceScores;
        }

        public IList<OtherViolationTypeModel> GetOtherViolationType(GetOtherViolationTypeCriteria criteria)
        {
            Check.Argument.IsNotNull(criteria, "criteria");
            IList<OtherViolationTypeModel> violationTypes = new List<OtherViolationTypeModel>();

            if (criteria.IsValid())
            {
                violationTypes = GetData<OtherViolationTypeModel>(criteria).ToList();
            }

            return violationTypes;
        }

        public void ExportAttendanceSummaryReport(
            GetAttendanceSummaryCriteria criteria,
            IEnumerable<AttendanceSummaryModel> attendanceEmployees,
            string templatePath)
        {
            string reportName = Lang.AttendanceSummaryReportName;
            string reportTitle = string.Empty;

            string startDate = criteria.StartDate.ToString(Constants.DateFormat);
            string endDate = criteria.EndDate.ToString(Constants.DateFormat);

            if (criteria.EmployeeId != null)
            {
                reportTitle = string.Format(Lang.AttendanceSummaryReportTitle, criteria.EmployeeName, startDate, endDate);
            }
            else if (criteria.DepartmentId != null)
            {
                reportTitle = string.Format(Lang.AttendanceSummaryReportTitle, criteria.DepartmentName, startDate, endDate);
            }
            else
            {
                reportTitle = string.Format(Lang.AttendanceSummaryReportTitleNoPrefix, startDate, endDate);
            }

            var writer = new ExcelTemplateWriter(templatePath, reportName, Lang.AttendanceSummaryReportSheet);

            // Fill report title.
            writer.WriteRow(0, new object[] { reportTitle });
            writer.CopyRows(1, 1);

            // Fill report header.
            writer.CopyRows(3, 1);

            FillContentSummaryRow(attendanceEmployees, ref writer);
            writer.SyncColumnsWidth(17);

            writer.Render();
        }

        public void ExportAttendanceDetailReport(
            GetDetailAttendanceCriteria criteria,
            IEnumerable<AttendanceDetailModel> attendanceViolations,
            string templatePath)
        {
            string reportName = Lang.AttendanceDetailReportName;
            string reportTitle = string.Empty;

            string startDate = criteria.StartDate.ToString(Constants.DateFormat);
            string endDate = criteria.EndDate.ToString(Constants.DateFormat);

            if (criteria.EmployeeId != null)
            {
                reportTitle = string.Format(Lang.AttendanceDetailReportTitle, criteria.EmployeeName, startDate, endDate);
            }
            else if (criteria.DepartmentId != null)
            {
                reportTitle = string.Format(Lang.AttendanceDetailReportTitle, criteria.DepartmentName, startDate, endDate);
            }
            else
            {
                reportTitle = string.Format(Lang.AttendanceDetailReportTitleNoPrefix, startDate, endDate);
            }

            var writer = new ExcelTemplateWriter(templatePath, reportName, Lang.AttendanceDetailReportSheet);

            // Fill report title.
            writer.WriteRow(0, new object[] { reportTitle });
            writer.CopyRows(1, 1);

            // Fill report header.
            writer.CopyRows(3, 1);

            FillContentDetailRow(attendanceViolations, ref writer);
            writer.SyncColumnsWidth(20);

            writer.Render();
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
            IEnumerable<AttendanceDetailModel> attendanceViolations =
                violations.GroupBy(x => new { x.EmployeeId, x.WorkDate, x.ShiftName, x.OutOfDate, x.InFixed, x.OutFixed })
                .Select(y =>
                {
                    List<AttendanceDetailModel> attendances = y.ToList();
                    var attendance = new AttendanceDetailModel
                        {
                            EmployeeId = y.Key.EmployeeId,
                            WorkDate = y.Key.WorkDate,
                            ShiftName = y.Key.ShiftName,
                            OutOfDate = y.Key.OutOfDate,
                            InFixed = y.Key.InFixed,
                            OutFixed = y.Key.OutFixed,
                            MissingCheckInOut = GetMissingCheckInOut(attendances),
                            TotalHours = GetTotalHours(attendances),
                            LateIn = GetLateIn(attendances),
                            EarlyOut = GetEarlyOut(attendances),
                            Absent = GetAbsent(attendances),
                            UnpaidLeaveAbsent = GetUnpaidLeaveAbsent(attendances),
                            UnpaidLeave = GetUnpaidLeave(attendances),
                            ChangeShift = GetChangeShift(attendances)
                        };
                    return attendance;
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

                if (commentItems != null && commentItems.Any())
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
            var violations = new List<ViolationModel>
            {
                attendanceDetail.MissingCheckInOut,
                attendanceDetail.LateIn,
                attendanceDetail.EarlyOut,
                attendanceDetail.TotalHours,
                attendanceDetail.Absent,
                attendanceDetail.UnpaidLeave,
                attendanceDetail.UnpaidLeaveAbsent,
                attendanceDetail.ChangeShift
            };

            foreach (ViolationModel violation in violations)
            {
                if (violation != null && violation.HasViolated)
                {
                    attendanceDetail.SystemSubtractScore += violation.Score;
                    attendanceDetail.ApproverSubtractScore += !violation.HasIgnored ? violation.Score : 0;
                }
            }

            if (attendanceDetail.SystemSubtractScore > Constants.MaxScoreSubtractDate)
            {
                attendanceDetail.SystemSubtractScore = Constants.MaxScoreSubtractDate;
            }
        }

        private static IEnumerable<AttendanceDetailModel> GroupEmployeeInformation(
            IEnumerable<AttendanceDetailModel> attendanceViolations,
            IEnumerable<EmployeeModel> employees)
        {
            foreach (AttendanceDetailModel attendanceViolation in attendanceViolations)
            {
                attendanceViolation.Employee = employees.FirstOrDefault(x => x.EmployeeId == attendanceViolation.EmployeeId);
            }

            return attendanceViolations;
        }

        private static ViolationModel GetMissingCheckInOut(IEnumerable<AttendanceDetailModel> groupAttendances)
        {
            var missingCheckInOut = new ViolationModel();
            AttendanceDetailModel attendanceDetail = groupAttendances.FirstOrDefault(x => x.MissingCheckInOut != null);

            if (attendanceDetail != null)
            {
                missingCheckInOut = attendanceDetail.MissingCheckInOut;
            }

            return missingCheckInOut;
        }

        private static ViolationModel GetTotalHours(List<AttendanceDetailModel> groupAttendances)
        {
            IEnumerable<AttendanceDetailModel> totalHours = groupAttendances.Where(x => x.TotalHours != null);
            var totalHour = new ViolationModel();

            if (totalHours != null && totalHours.Any())
            {
                totalHour = totalHours.First().TotalHours;
            }

            return totalHour;
        }

        private static ViolationModel GetLateIn(List<AttendanceDetailModel> groupAttendances)
        {
            IEnumerable<AttendanceDetailModel> lateIns = groupAttendances.Where(x => x.LateIn != null);
            var lateIn = new ViolationModel();

            if (lateIns != null && lateIns.Any())
            {
                lateIn = lateIns.First().LateIn;
            }

            return lateIn;
        }

        private static ViolationModel GetEarlyOut(List<AttendanceDetailModel> groupAttendances)
        {
            IEnumerable<AttendanceDetailModel> earlyOuts = groupAttendances.Where(x => x.EarlyOut != null);
            var earlyOut = new ViolationModel();

            if (earlyOuts != null && earlyOuts.Any())
            {
                earlyOut = earlyOuts.First().EarlyOut;
            }

            return earlyOut;
        }

        private static ViolationModel GetAbsent(List<AttendanceDetailModel> groupAttendances)
        {
            IEnumerable<AttendanceDetailModel> absents = groupAttendances.Where(x => x.Absent != null);
            var absent = new ViolationModel();

            if (absents != null && absents.Any())
            {
                absent = absents.First().Absent;
            }

            return absent;
        }

        private static ViolationModel GetUnpaidLeaveAbsent(List<AttendanceDetailModel> groupAttendances)
        {
            IEnumerable<AttendanceDetailModel> unpaidLeaveAbsents = groupAttendances.Where(x => x.UnpaidLeaveAbsent != null);
            var unpaidLeaveAbsent = new ViolationModel();

            if (unpaidLeaveAbsents != null && unpaidLeaveAbsents.Any())
            {
                unpaidLeaveAbsent = unpaidLeaveAbsents.First().UnpaidLeaveAbsent;
            }

            return unpaidLeaveAbsent;
        }

        private static ViolationModel GetUnpaidLeave(List<AttendanceDetailModel> groupAttendances)
        {
            IEnumerable<AttendanceDetailModel> unpaidLeaves = groupAttendances.Where(x => x.UnpaidLeave != null);
            var unpaidLeave = new ViolationModel();

            if (unpaidLeaves != null && unpaidLeaves.Any())
            {
                unpaidLeave = unpaidLeaves.First().UnpaidLeave;
            }

            return unpaidLeave;
        }

        private static ViolationModel GetChangeShift(List<AttendanceDetailModel> groupAttendances)
        {
            IEnumerable<AttendanceDetailModel> changeShifts = groupAttendances.Where(x => x.ChangeShift != null);
            var changeShift = new ViolationModel();

            if (changeShifts != null && changeShifts.Any())
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
                string[] comments = comment.Split('&');

                foreach (string commentItem in comments)
                {
                    string[] commentItems = commentItem.Split('_');

                    if (commentItems != null && commentItems.Count() > 1 && !string.IsNullOrWhiteSpace(commentItems[0]) && !string.IsNullOrWhiteSpace(commentItems[1]))
                    {
                        approverComment += commentItems[0] + ':' + commentItems[1];
                    }
                }
            }

            return approverComment;
        }

        private void FillContentSummaryRow(IEnumerable<AttendanceSummaryModel> attendanceEmployees, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (AttendanceSummaryModel attendanceEmployee in attendanceEmployees)
            {
                FillSummaryRow(attendanceEmployee, ref writer, ref rowIndex);
            }
        }

        private void FillSummaryRow(AttendanceSummaryModel attendanceEmployee, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateSummaryRecord(attendanceEmployee, ref rowIndex);
            writer.WriteRow(4, record);
        }

        private object CreateSummaryRecord(AttendanceSummaryModel attendanceEmployee, ref int rowIndex)
        {
            float otherViolations = attendanceEmployee.OtherViolationsScore > 0 ? -attendanceEmployee.OtherViolationsScore : 0;
            float systemSubtractScore = attendanceEmployee.SystemSubtractScore > 0 ? -attendanceEmployee.SystemSubtractScore : 0;
            float approverSubtractScore = attendanceEmployee.ApproverSubtractScore > 0 ? -attendanceEmployee.ApproverSubtractScore : 0;

            var record = new
            {
                Count = rowIndex,
                Section = attendanceEmployee.Group != null ? attendanceEmployee.Group.GroupName : string.Empty,
                Code = attendanceEmployee.Employee.Code,
                Nick = attendanceEmployee.Employee.NickName,
                FullName = attendanceEmployee.Employee.FullName,
                Category = attendanceEmployee.Employee.Category,
                MissingCheckInOut = attendanceEmployee.MissingCheckInOut,
                WorkingBelowTime = attendanceEmployee.WorkingBelowTime,
                LateIn = attendanceEmployee.LateIn,
                EarlyOut = attendanceEmployee.EarlyOut,
                Absent = attendanceEmployee.Absent,
                UnpaidLeave = attendanceEmployee.UnpaidLeave,
                UnpaidLeaveAbsent = attendanceEmployee.UnpaidLeaveAbsent,
                ChangeShift = attendanceEmployee.ChangeShift,
                OtherViolation = (decimal)otherViolations,
                SystemSubtractScore = (decimal)systemSubtractScore,
                ApproverSubtractScore = (decimal)approverSubtractScore
            };
            rowIndex++;

            return record;
        }

        private void FillContentDetailRow(IEnumerable<AttendanceDetailModel> attendanceViolations, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (AttendanceDetailModel attendanceViolation in attendanceViolations)
            {
                FillDetailRow(attendanceViolation, ref writer, ref rowIndex);
            }
        }

        private void FillDetailRow(AttendanceDetailModel attendanceViolation, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateDetailRecord(attendanceViolation, ref rowIndex);
            writer.WriteRow(4, record);
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2100:CodeLineMustNotBeLongerThan", Justification = "Reviewed. Suppression is OK here.")]
        private object CreateDetailRecord(AttendanceDetailModel attendanceViolation, ref int rowIndex)
        {
            float systemSubtractScore = attendanceViolation.SystemSubtractScore > 0 ? -attendanceViolation.SystemSubtractScore : 0;
            float approverSubtractScore = attendanceViolation.ApproverSubtractScore > 0 ? -attendanceViolation.ApproverSubtractScore : 0;
            float totalHours = attendanceViolation.TotalHours.Value != null ? float.Parse(attendanceViolation.TotalHours.Value) * 60 : 0;

            var record = new
            {
                Shift = attendanceViolation.Employee.Code,
                Nick = attendanceViolation.Employee.NickName,
                FullName = attendanceViolation.Employee.FullName,
                Category = attendanceViolation.Employee.Category,
                Section = attendanceViolation.Employee.Group.GroupName,
                Date = attendanceViolation.WorkDate,
                Code = attendanceViolation.ShiftName,
                CheckIn = !string.IsNullOrWhiteSpace(attendanceViolation.InFixed) ? DateTime.Parse(attendanceViolation.InFixed).ToString(Constants.TimeFormant) : string.Empty,
                CheckOut = !string.IsNullOrWhiteSpace(attendanceViolation.OutFixed) ? DateTime.Parse(attendanceViolation.OutFixed).ToString(Constants.TimeFormant) : string.Empty,
                TotalHours = !string.IsNullOrWhiteSpace(attendanceViolation.TotalHours.Value) ? ConvertNumberToTime(totalHours) : string.Empty,
                MissingCheckInOut = !string.IsNullOrWhiteSpace(attendanceViolation.MissingCheckInOut.Value) ? attendanceViolation.MissingCheckInOut.Value : string.Empty,
                WorkingUnderTime = !string.IsNullOrWhiteSpace(attendanceViolation.TotalHours.Value) && attendanceViolation.TotalHours.HasViolated ? "1" : string.Empty,
                LateIn = !string.IsNullOrWhiteSpace(attendanceViolation.LateIn.Value) ? ConvertNumberToTime(double.Parse(attendanceViolation.LateIn.Value)) : string.Empty,
                EarlyOut = !string.IsNullOrWhiteSpace(attendanceViolation.EarlyOut.Value) ? ConvertNumberToTime(double.Parse(attendanceViolation.EarlyOut.Value)) : string.Empty,
                Absent = !string.IsNullOrWhiteSpace(attendanceViolation.Absent.Value) ? attendanceViolation.Absent.Value : string.Empty,
                UnpaidLeave = !string.IsNullOrWhiteSpace(attendanceViolation.UnpaidLeave.Value) ? attendanceViolation.UnpaidLeave.Value : string.Empty,
                UnpaidLeaveAbsent = !string.IsNullOrWhiteSpace(attendanceViolation.UnpaidLeaveAbsent.Value) ? attendanceViolation.UnpaidLeaveAbsent.Value : string.Empty,
                ChangeShift = !string.IsNullOrWhiteSpace(attendanceViolation.ChangeShift.Value) ? attendanceViolation.ChangeShift.Value : string.Empty,
                SystemSubtractScore = (decimal)systemSubtractScore,
                ApproverSubtractScore = (decimal)approverSubtractScore,
                ApproverComment = attendanceViolation.ApproverComment,
                EmployeeComment = attendanceViolation.EmployeeComment
            };
            rowIndex++;

            return record;
        }

        private string ConvertNumberToTime(double number)
        {
            double numberMinutes = Math.Round(number);

            var timeSpan = new TimeSpan(0, (int)numberMinutes, 0);
            string time = timeSpan.ToString(@"hh\:mm");

            return time;
        }
    }
}