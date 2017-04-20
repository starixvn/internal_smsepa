namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories;

    using SMS.EPA.Resources;
    using SMS.EPA.Services.Interfaces;

    public class DisciplineService : BaseService, IDisciplineService
    {
        private readonly IDisciplineSummaryRepository disciplineSummaryRepository;

        public DisciplineService()
            : this(null)
        {
        }

        public DisciplineService(IDisciplineSummaryRepository disciplineSummaryRepository)
        {
            this.disciplineSummaryRepository = disciplineSummaryRepository ?? new DisciplineSummaryRepository();
        }

        public IEnumerable<DisciplineSummaryModel> GetDisciplineSummary(GetDisciplineSummaryCriteria criteria)
        {
            IEnumerable<DisciplineSummaryModel> employeeDisciplines = new List<DisciplineSummaryModel>();

            if (criteria != null && criteria.IsValid())
            {
                employeeDisciplines = disciplineSummaryRepository.GetDisciplineSummary(criteria);
                employeeDisciplines = employeeDisciplines.OrderByDescending(x => x.Permission);
            }

            return employeeDisciplines;
        }

        public IEnumerable<OtherViolationModel> GetOtherViolations(GetOtherViolationsByEmployeeIdCriteria criteria)
        {
            IEnumerable<OtherViolationModel> violations = new List<OtherViolationModel>();

            if (criteria != null && criteria.IsValid())
            {
                violations = this.GetData<OtherViolationModel>(criteria);
            }

            return violations;
        }

        public IEnumerable<DisciplineTypeModel> GetDisciplineType(GetDisciplineTypeCriteria criteria)
        {
            IEnumerable<DisciplineTypeModel> violations = new List<DisciplineTypeModel>();

            if (criteria != null && criteria.IsValid())
            {
                violations = this.GetData<DisciplineTypeModel>(criteria);
            }

            return violations;
        }

        public IEnumerable<DisciplineDetailModel> GetDisciplineDetail(GetDisciplineDetailCriteria criteria)
        {
            IEnumerable<DisciplineDetailModel> employeeDisciplines = new List<DisciplineDetailModel>();

            if (criteria != null && criteria.IsValid())
            {
                employeeDisciplines = disciplineSummaryRepository.GetDisciplineDetail(criteria);
                employeeDisciplines = employeeDisciplines.OrderBy(x => x.Employee.Group.GroupId)
                    .ThenBy(x => x.Employee.Code)
                    .ThenByDescending(x => x.WorkDate);
            }

            return employeeDisciplines;
        }

        public IEnumerable<DisciplineSummaryByPeriodModel> GetDisciplineSummaryByPeriod(GetDisciplineSummaryByPeriodCriteria criteria)
        {
            IEnumerable<DisciplineSummaryByPeriodModel> employeeDisciplines = new List<DisciplineSummaryByPeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                employeeDisciplines = disciplineSummaryRepository.GetDisciplineSummaryByPeriod(criteria);
            }

            return employeeDisciplines;
        }

        public void ExportDisciplineSummaryReport(
            GetDisciplineSummaryCriteria criteria,
            IEnumerable<DisciplineSummaryModel> employeeDisciplines,
            string templatePath)
        {
            if (criteria != null)
            {
                string reportName = Lang.AttendanceSummaryReportName;
                string reportTitle;
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

                FillContentSummaryRow(employeeDisciplines, ref writer);
                writer.SyncColumnsWidth(19);
                writer.CreateFreezePane(0, 3);

                writer.Render();
            }
        }

        public void ExportDisciplineDetailReport(
            GetDisciplineDetailCriteria criteria,
            IEnumerable<DisciplineDetailModel> employeeDisciplines,
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

                FillContentDetailRow(employeeDisciplines, ref writer);
                writer.SyncColumnsWidth(24);
                writer.CreateFreezePane(0, 3);

                writer.Render();
            }
        }

        public void ExportDisciplineSummaryByPeriodReport(
            GetDisciplineSummaryByPeriodCriteria criteria,
            IEnumerable<DisciplineSummaryByPeriodModel> employeeDisciplines,
            string templatePath)
        {
            if (criteria != null)
            {
                string reportTitle = string.Empty;

                if (criteria.EmployeeId != null)
                {
                    reportTitle = string.Format("{0} - Discipline Summary Report", criteria.EmployeeName);
                }
                else if (criteria.DepartmentId != null)
                {
                    reportTitle = string.Format("{0} - Discipline Summary Report", criteria.DepartmentName);
                }
                else
                {
                    reportTitle = "Discipline Summary Report";
                }

                var writer = new ExcelTemplateWriter(templatePath, "Discipline_Summary_Report", "Discipline Summary");

                // Fill report title.
                writer.WriteRow(0, new object[] { reportTitle });
                writer.CopyRows(1, 1);

                // Fill report header.
                writer.CopyRows(2, 1);

                FillContentRowDisciplineSummaryByPeriod(employeeDisciplines, ref writer);
                writer.SyncColumnsWidth(15);
                writer.CreateFreezePane(0, 3);

                writer.Render();
            }
        }

        private void FillContentSummaryRow(IEnumerable<DisciplineSummaryModel> employeeDisciplines, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (DisciplineSummaryModel employeeDiscipline in employeeDisciplines)
            {
                FillSummaryRow(employeeDiscipline, ref writer, ref rowIndex);
            }
        }

        private void FillSummaryRow(DisciplineSummaryModel employeeDiscipline, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateSummaryRecord(employeeDiscipline, ref rowIndex);
            writer.WriteRow(4, record);
        }

        private object CreateSummaryRecord(DisciplineSummaryModel employeeDiscipline, ref int rowIndex)
        {
            float otherViolations = employeeDiscipline.OtherViolationsScore > 0 ? -employeeDiscipline.OtherViolationsScore : 0,
                systemSubtractScore = employeeDiscipline.SystemSubtractScore > 0 ? -employeeDiscipline.SystemSubtractScore : 0,
                approverSubtractScore = employeeDiscipline.ApproverSubtractScore > 0 ? -employeeDiscipline.ApproverSubtractScore : 0;

            var record = new
            {
                Count = rowIndex,
                Section = employeeDiscipline.Employee.Group != null ? employeeDiscipline.Employee.Group.GroupName : string.Empty,
                Code = employeeDiscipline.Employee.Code,
                Nick = employeeDiscipline.Employee.NickName,
                FullName = employeeDiscipline.Employee.FullName,
                JobTitle = employeeDiscipline.Employee.JobTitle != null ? employeeDiscipline.Employee.JobTitle.JobTitleName : string.Empty,
                JoinedDate = employeeDiscipline.Employee.JoinedDate.ToString(Constants.DateFormat),
                Category = employeeDiscipline.Employee.Category,
                MissingCheckInOut = employeeDiscipline.MissingCheckInOut,
                WorkingUnderTime = employeeDiscipline.WorkingUnderTime,
                LateIn = employeeDiscipline.LateIn,
                EarlyOut = employeeDiscipline.EarlyOut,
                Absent = employeeDiscipline.Absent,
                UnpaidLeave = employeeDiscipline.UnpaidLeave,
                UnpaidLeaveAbsent = employeeDiscipline.UnpaidLeaveAbsent,
                ChangeShift = employeeDiscipline.ChangeShift,
                OtherViolation = (decimal)otherViolations,
                SystemSubtractScore = (decimal)systemSubtractScore,
                ApproverSubtractScore = (decimal)approverSubtractScore
            };
            rowIndex++;

            return record;
        }

        private void FillContentDetailRow(IEnumerable<DisciplineDetailModel> employeeDisciplines, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (DisciplineDetailModel employeeDiscipline in employeeDisciplines)
            {
                FillDetailRow(employeeDiscipline, ref writer, ref rowIndex);
            }
        }

        private void FillDetailRow(DisciplineDetailModel employeeDiscipline, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateDetailRecord(employeeDiscipline, ref rowIndex);

            if (employeeDiscipline.IsWeekend)
            {
                writer.WriteRow(5, record);
            }
            else
            {
                writer.WriteRow(4, record);
            }
        }

        private object CreateDetailRecord(DisciplineDetailModel employeeDiscipline, ref int rowIndex)
        {
            decimal systemSubtractScore = employeeDiscipline.SystemSubtractScore > 0 ? -employeeDiscipline.SystemSubtractScore : 0,
                approverSubtractScore = employeeDiscipline.ApproverSubtractScore > 0 ? -employeeDiscipline.ApproverSubtractScore : 0;

            var record = new
            {
                Shift = employeeDiscipline.Employee.Code,
                Nick = employeeDiscipline.Employee.NickName,
                FullName = employeeDiscipline.Employee.FullName,
                Section = employeeDiscipline.Employee.Group != null ? employeeDiscipline.Employee.Group.GroupName : string.Empty,
                JobTitle = employeeDiscipline.Employee.JobTitle != null ? employeeDiscipline.Employee.JobTitle.JobTitleName : string.Empty,
                JoinedDate = employeeDiscipline.Employee.JoinedDate.ToString(Constants.DateFormat),
                Category = employeeDiscipline.Employee.Category,
                Date = employeeDiscipline.WorkDateToString,
                Code = employeeDiscipline.ShiftName,
                CheckIn = employeeDiscipline.CheckIn.HasValue ? employeeDiscipline.CheckIn.Value.ToString(Constants.TimeFormat) : string.Empty,
                CheckOut = employeeDiscipline.CheckOut.HasValue ? employeeDiscipline.CheckOut.Value.ToString(Constants.TimeFormat) : string.Empty,
                TotalHours = !string.IsNullOrEmpty(employeeDiscipline.TotalHours) ? employeeDiscipline.TotalHours : string.Empty,
                MissingCheckInOut = employeeDiscipline.MissingCheckInOut.Value > 0 ? employeeDiscipline.MissingCheckInOut.Value.ToString() : string.Empty,
                WorkingUnderTime = employeeDiscipline.WorkingUnderTime.Value > 0 ? "1" : string.Empty,
                LateIn = !string.IsNullOrEmpty(employeeDiscipline.NumberOfMinutesLateIn) ? employeeDiscipline.NumberOfMinutesLateIn : string.Empty,
                EarlyOut = !string.IsNullOrEmpty(employeeDiscipline.NumberOfMinutesEarlyOut) ? employeeDiscipline.NumberOfMinutesEarlyOut : string.Empty,
                Absent = employeeDiscipline.Absent.Value > 0 ? employeeDiscipline.Absent.Value.ToString() : string.Empty,
                UnpaidLeave = employeeDiscipline.UnpaidLeave.Value > 0 ? employeeDiscipline.UnpaidLeave.Value.ToString() : string.Empty,
                UnpaidLeaveAbsent = employeeDiscipline.UnpaidLeaveAbsent.Value > 0 ? employeeDiscipline.UnpaidLeaveAbsent.Value.ToString() : string.Empty,
                ChangeShift = employeeDiscipline.ChangeShift.Value > 0 ? employeeDiscipline.ChangeShift.Value.ToString() : string.Empty,
                SystemSubtractScore = (decimal)systemSubtractScore,
                ApproverSubtractScore = (decimal)approverSubtractScore,
                ApproverComment = employeeDiscipline.ApproverComment,
                EmployeeComment = employeeDiscipline.EmployeeComment
            };
            rowIndex++;

            return record;
        }

        private void FillContentRowDisciplineSummaryByPeriod(IEnumerable<DisciplineSummaryByPeriodModel> employeeDisciplines, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (DisciplineSummaryByPeriodModel employeeDiscipline in employeeDisciplines)
            {
                FillRowDisciplineSummaryByPeriod(employeeDiscipline, ref writer, ref rowIndex);
            }
        }

        private void FillRowDisciplineSummaryByPeriod(DisciplineSummaryByPeriodModel employeeDiscipline, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordDisciplineSummaryByPeriod(employeeDiscipline, ref rowIndex);
            writer.WriteRow(3, record);
        }

        private object CreateRecordDisciplineSummaryByPeriod(DisciplineSummaryByPeriodModel employeeDiscipline, ref int rowIndex)
        {
            decimal missingCheckInOut = employeeDiscipline.MissingCheckInOut > 0 ? -employeeDiscipline.MissingCheckInOut : 0,
                lateIn = employeeDiscipline.LateIn > 0 ? -employeeDiscipline.LateIn : 0,
                unpaidLeave = employeeDiscipline.UnpaidLeave > 0 ? -employeeDiscipline.UnpaidLeave : 0,
                workingUndertime = employeeDiscipline.WorkingUndertime > 0 ? -employeeDiscipline.WorkingUndertime : 0,
                otherDisciplines = employeeDiscipline.OtherDisciplines > 0 ? -employeeDiscipline.OtherDisciplines : 0,
                approverSubtractScore = employeeDiscipline.ApproverSubtractScore > 0 ? -employeeDiscipline.ApproverSubtractScore : 0;

            var record = new
            {
                Count = rowIndex,
                Period = employeeDiscipline.Period.Name,
                Code = employeeDiscipline.Employee.Code,
                Nick = employeeDiscipline.Employee.NickName,
                FullName = employeeDiscipline.Employee.FullName,
                Section = employeeDiscipline.Employee.Group != null ? employeeDiscipline.Employee.Group.GroupName : string.Empty,
                JobTitle = employeeDiscipline.Employee.JobTitle != null ? employeeDiscipline.Employee.JobTitle.JobTitleName : string.Empty,
                JoinedDate = employeeDiscipline.Employee.JoinedDate.ToString(Constants.DateFormat),
                Category = employeeDiscipline.Employee.Category,
                MissingCheckInOut = missingCheckInOut,
                LateIn = lateIn,
                UnpaidLeave = unpaidLeave,
                WorkingUndertime = workingUndertime,
                OtherDisciplines = otherDisciplines,
                ApproverSubtractScore = approverSubtractScore
            };
            rowIndex++;

            return record;
        }
    }
}