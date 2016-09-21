namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class DisciplineSummaryService : BaseService, IDisciplineSummaryService
    {
        private readonly IDisciplineSummaryRepository disciplineSummaryRepository;

        public DisciplineSummaryService()
            : this(null)
        {
        }

        public DisciplineSummaryService(IDisciplineSummaryRepository disciplineSummaryRepository)
        {
            this.disciplineSummaryRepository = disciplineSummaryRepository ?? new DisciplineSummaryRepository();
        }

        public IEnumerable<DisciplineSummaryModel> GetDisciplineSummaryByPeriod(GetDisciplineSummaryByPeriodCriteria criteria)
        {
            IEnumerable<DisciplineSummaryModel> employeeDisciplines = new List<DisciplineSummaryModel>();

            if (criteria != null && criteria.IsValid())
            {
                employeeDisciplines = disciplineSummaryRepository.GetDisciplineSummaryByPeriod(criteria);
            }

            return employeeDisciplines;
        }

        public void ExportDisciplineSummaryByPeriodReport(
            GetDisciplineSummaryByPeriodCriteria criteria,
            IEnumerable<DisciplineSummaryModel> employeeDisciplines,
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

        private void FillContentRowDisciplineSummaryByPeriod(IEnumerable<DisciplineSummaryModel> employeeDisciplines, ref ExcelTemplateWriter writer)
        {
            int rowIndex = 1;

            foreach (DisciplineSummaryModel employeeDiscipline in employeeDisciplines)
            {
                FillRowDisciplineSummaryByPeriod(employeeDiscipline, ref writer, ref rowIndex);
            }
        }

        private void FillRowDisciplineSummaryByPeriod(DisciplineSummaryModel employeeDiscipline, ref ExcelTemplateWriter writer, ref int rowIndex)
        {
            object record = CreateRecordDisciplineSummaryByPeriod(employeeDiscipline, ref rowIndex);
            writer.WriteRow(3, record);
        }

        private object CreateRecordDisciplineSummaryByPeriod(DisciplineSummaryModel employeeDiscipline, ref int rowIndex)
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