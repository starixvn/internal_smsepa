namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Interfaces;

    public class AttendanceSummaryService : BaseService, IAttendanceSummaryService
    {
        private readonly IAttendanceSummaryRepository _attendanceSummaryRepository;

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
            IEnumerable<AttendanceSummaryModel> attendanceEmployees = new List<AttendanceSummaryModel>();

            if (criteria != null && criteria.IsValid())
            {
                attendanceEmployees = _attendanceSummaryRepository.GetAttendanceSummary(criteria);
                attendanceEmployees = attendanceEmployees.OrderByDescending(x => x.Permission);
            }

            return attendanceEmployees;
        }

        public IEnumerable<OtherViolationModel> GetOtherViolations(GetOtherViolationsOfEmployeeCriteria criteria)
        {
            IEnumerable<OtherViolationModel> violations = new List<OtherViolationModel>();

            if (criteria != null && criteria.IsValid())
            {
                violations = GetData<OtherViolationModel>(criteria);
            }

            return violations;
        }

        public IEnumerable<ViolationTypeModel> GetViolationType(GetViolationTypeCriteria criteria)
        {
            IEnumerable<ViolationTypeModel> violations = new List<ViolationTypeModel>();

            if (criteria != null && criteria.IsValid())
            {
                violations = GetData<ViolationTypeModel>(criteria);
            }

            return violations;
        }

        public IEnumerable<AttendanceDetailModel> GetAttendanceDetail(GetDetailAttendanceCriteria criteria)
        {
            IEnumerable<AttendanceDetailModel> attendanceViolations = new List<AttendanceDetailModel>();

            if (criteria != null && criteria.IsValid())
            {
                attendanceViolations = _attendanceSummaryRepository.GetAttendanceDetail(criteria);
                attendanceViolations = attendanceViolations.OrderBy(x => x.Employee.Group.GroupId)
                    .ThenBy(x => x.Employee.Code)
                    .ThenByDescending(x => x.WorkDate);
            }

            return attendanceViolations;
        }

        public void ExportAttendanceSummaryReport(
            GetAttendanceSummaryCriteria criteria,
            IEnumerable<AttendanceSummaryModel> attendanceEmployees,
            string templatePath)
        {
            if (criteria != null)
            {
                string reportName = Lang.AttendanceSummaryReportName,
                    reportTitle = string.Empty,
                    startDate = criteria.StartDate.ToString(Constants.DateFormat),
                    endDate = criteria.EndDate.ToString(Constants.DateFormat);

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
        }

        public void ExportAttendanceDetailReport(
            GetDetailAttendanceCriteria criteria,
            IEnumerable<AttendanceDetailModel> attendanceViolations,
            string templatePath)
        {
            if (criteria != null)
            {
                string reportName = Lang.AttendanceDetailReportName,
                    reportTitle = string.Empty,
                    startDate = criteria.StartDate.ToString(Constants.DateFormat),
                    endDate = criteria.EndDate.ToString(Constants.DateFormat);

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
            float otherViolations = attendanceEmployee.OtherViolationsScore > 0 ? -attendanceEmployee.OtherViolationsScore : 0,
                systemSubtractScore = attendanceEmployee.SystemSubtractScore > 0 ? -attendanceEmployee.SystemSubtractScore : 0,
                approverSubtractScore = attendanceEmployee.ApproverSubtractScore > 0 ? -attendanceEmployee.ApproverSubtractScore : 0;

            var record = new
            {
                Count = rowIndex,
                Section = attendanceEmployee.Employee.Group != null ? attendanceEmployee.Employee.Group.GroupName : string.Empty,
                Code = attendanceEmployee.Employee.Code,
                Nick = attendanceEmployee.Employee.NickName,
                FullName = attendanceEmployee.Employee.FullName,
                JobTitle = attendanceEmployee.Employee.JobTitle != null ? attendanceEmployee.Employee.JobTitle.JobTitleName : string.Empty,
                JoinedDate = attendanceEmployee.Employee.JoinedDate.ToString(Constants.DateFormat),
                Category = attendanceEmployee.Employee.Category,
                MissingCheckInOut = attendanceEmployee.MissingCheckInOut,
                WorkingUnderTime = attendanceEmployee.WorkingUnderTime,
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

        private object CreateDetailRecord(AttendanceDetailModel attendanceViolation, ref int rowIndex)
        {
            decimal systemSubtractScore = attendanceViolation.SystemSubtractScore > 0 ? -attendanceViolation.SystemSubtractScore : 0,
                approverSubtractScore = attendanceViolation.ApproverSubtractScore > 0 ? -attendanceViolation.ApproverSubtractScore : 0;

            var record = new
            {
                Shift = attendanceViolation.Employee.Code,
                Nick = attendanceViolation.Employee.NickName,
                FullName = attendanceViolation.Employee.FullName,
                Section = attendanceViolation.Employee.Group != null ? attendanceViolation.Employee.Group.GroupName : string.Empty,
                JobTitle = attendanceViolation.Employee.JobTitle != null ? attendanceViolation.Employee.JobTitle.JobTitleName : string.Empty,
                JoinedDate = attendanceViolation.Employee.JoinedDate.ToString(Constants.DateFormat),
                Category = attendanceViolation.Employee.Category,
                Date = attendanceViolation.WorkDate,
                Code = attendanceViolation.ShiftName,
                CheckIn = attendanceViolation.CheckIn.HasValue ? attendanceViolation.CheckIn.Value.ToString(Constants.TimeFormat) : string.Empty,
                CheckOut = attendanceViolation.CheckOut.HasValue ? attendanceViolation.CheckOut.Value.ToString(Constants.TimeFormat) : string.Empty,
                TotalHours = !string.IsNullOrEmpty(attendanceViolation.TotalHours) ? attendanceViolation.TotalHours : string.Empty,
                MissingCheckInOut = attendanceViolation.MissingCheckInOut.Value > 0 ? attendanceViolation.MissingCheckInOut.Value.ToString() : string.Empty,
                WorkingUnderTime = attendanceViolation.WorkingUnderTime.Value > 0 ? "1" : string.Empty,
                LateIn = !string.IsNullOrEmpty(attendanceViolation.NumberOfMinutesLateIn) ? attendanceViolation.NumberOfMinutesLateIn : string.Empty,
                EarlyOut = !string.IsNullOrEmpty(attendanceViolation.NumberOfMinutesEarlyOut) ? attendanceViolation.NumberOfMinutesEarlyOut : string.Empty,
                Absent = attendanceViolation.Absent.Value > 0 ? attendanceViolation.Absent.Value.ToString() : string.Empty,
                UnpaidLeave = attendanceViolation.UnpaidLeave.Value > 0 ? attendanceViolation.UnpaidLeave.Value.ToString() : string.Empty,
                UnpaidLeaveAbsent = attendanceViolation.UnpaidLeaveAbsent.Value > 0 ? attendanceViolation.UnpaidLeaveAbsent.Value.ToString() : string.Empty,
                ChangeShift = attendanceViolation.ChangeShift.Value > 0 ? attendanceViolation.ChangeShift.Value.ToString() : string.Empty,
                SystemSubtractScore = (decimal)systemSubtractScore,
                ApproverSubtractScore = (decimal)approverSubtractScore,
                ApproverComment = attendanceViolation.ApproverComment,
                EmployeeComment = attendanceViolation.EmployeeComment
            };
            rowIndex++;

            return record;
        }
    }
}