namespace SMS.EPA.Site.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Services;
    using Criteria;
    using Criteria.Report;
    using Model;
    using Model.Discipline;
    using Model.Enitites;
    using Model.Report;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using Resources;
    using Services.Implement;
    using Services.Implement.ExportFile;
    using Services.Interfaces;
    using Services.Interfaces.ExportFile;

    public class ExportReportController : Controller
    {
        private const int DefaultContentRowStartIndexForTemplate = 3;
        private const string DisciplineDetailTemplatePath = @"~\ExcelTemplates\DisciplineDetailReport.xls";
        private const string DisciplineSummaryByPeriodTemplatePath = @"~\ExcelTemplates\DisciplineSummaryByPeriodReport.xls";
        private const string DisciplineSummaryTemplatePath = @"~\ExcelTemplates\DisciplineSummaryReport.xls";
        private const string EmployeeBasicInfoTemplate = @"~\ExcelTemplates\EmployeeBasicInfoTemplate.xls";
        private const string EmployeeFlowSettingTemplate = @"~\ExcelTemplates\EmployeeFlowSettingTemplate.xls";
        private const string PersonalFlowSettingTemplate = @"~\ExcelTemplates\PersonalFlowSettingTemplate.xls";
        private const string TargetPersonnelSettingTemplate = @"~\ExcelTemplates\TargetPersonnelSettingTemplate.xls";
        private const string UpdateFinalScoreTemplatePath = @"~\ExcelTemplates\UpdateFinalScore.xls";
        private readonly IDisciplineService _disciplineService;
        private readonly IEmployeeEPAService _employeeEPAService;
        private readonly IEmployeeService _employeeService;
        private readonly IEPADetailReportService _epaDetailReportService;
        private readonly IExportExcelService _exportExcelService;
        private readonly IEPADetailReportService _monthlyEPADetailReportService;
        private readonly IPeriodService _periodService;
        private readonly IPersonalFlowService _personalFlowService;
        private readonly IReportService _reportService;
        private readonly ISessionService _sessionService;
        private readonly ITargetEmployeeSettingService _targetEmployeeSettingService;

        public ExportReportController()
            : this(null, null, null, null, null, null, null, null, null, null, null)
        {
        }

        public ExportReportController(
            IReportService reportService,
            IEPADetailReportService epaDetailReportService,
            IEPADetailReportService monthlyEPADetailReportService,
            IDisciplineService disciplineService,
            IEmployeeEPAService employeeEPAService,
            IExportExcelService exportExcelService,
            ITargetEmployeeSettingService targetEmployeeSettingService,
            IPeriodService periodService,
            IEmployeeService employeeService,
            IPersonalFlowService personalFlowService,
            ISessionService sessionService)
        {
            _reportService = reportService ?? new ReportService();
            _epaDetailReportService = epaDetailReportService ?? new EPADetailReportService();
            _monthlyEPADetailReportService = monthlyEPADetailReportService ?? new MonthlyEPADetailReportService();
            _disciplineService = disciplineService ?? new DisciplineService();
            _sessionService = sessionService ?? new SessionService();
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            _exportExcelService = exportExcelService ?? new ExportExcelService();
            _targetEmployeeSettingService = targetEmployeeSettingService ?? new TargetEmployeeSettingService();
            _periodService = periodService ?? new PeriodService();
            _employeeService = employeeService ?? new EmployeeService();
            _personalFlowService = personalFlowService ?? new PersonalFlowService();
        }

        public ActionResult ExportDisciplineDetailReport(GetDisciplineDetailCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ApproverId = _sessionService.CurrentUser.EmployeeId;
                IEnumerable<DisciplineDetailModel> employeeDisciplines =
                    _disciplineService.GetDisciplineDetail(criteria);

                if (employeeDisciplines != null && employeeDisciplines.Any())
                {
                    _exportExcelService.FileName = Lang.AttendanceDetailReportName;
                    _exportExcelService.SheetName = Lang.AttendanceDetailReportSheet;
                    _exportExcelService.Title = GetDisciplineDetailReportTitle(criteria);

                    _exportExcelService.InitializeWithTemplate(Server.MapPath(DisciplineDetailTemplatePath));

                    _exportExcelService.ContentRowStartIndex = employeeDisciplines.First().IsWeekend ? 4 : 5;

                    CreateStyleForDisciplineDetailReport(employeeDisciplines);

                    _exportExcelService.ExportWithTemplate(employeeDisciplines, true, true);
                }
            }

            return new EmptyResult();
        }

        public ActionResult ExportDisciplineSummaryByPeriodReport(GetDisciplineSummaryByPeriodCriteria criteria)
        {
            if (criteria != null)
            {
                IEnumerable<DisciplineSummaryByPeriodModel> employeeDisciplines = _disciplineService.GetDisciplineSummaryByPeriod(criteria);

                if (employeeDisciplines != null && employeeDisciplines.Any())
                {
                    _exportExcelService.FileName = "Discipline_Summary_Report";
                    _exportExcelService.SheetName = "Discipline Summary";
                    _exportExcelService.Title = GetDisciplineSummaryByPeriodReportTitle(criteria);

                    _exportExcelService.InitializeWithTemplate(Server.MapPath(DisciplineSummaryByPeriodTemplatePath));

                    _exportExcelService.ContentRowStartIndex = DefaultContentRowStartIndexForTemplate;

                    _exportExcelService.ExportWithTemplate(employeeDisciplines, true, true);
                }
            }

            return new EmptyResult();
        }

        public ActionResult ExportDisciplineSummaryReport(GetDisciplineSummaryCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ApproverId = _sessionService.CurrentUser.EmployeeId;
                IEnumerable<DisciplineSummaryModel> employeeDisciplines = _disciplineService.GetDisciplineSummary(criteria);

                if (employeeDisciplines != null && employeeDisciplines.Any())
                {
                    _exportExcelService.FileName = Lang.AttendanceSummaryReportName;
                    _exportExcelService.SheetName = Lang.AttendanceSummaryReportSheet;
                    _exportExcelService.Title = GetDisciplineSummaryReportTitle(criteria);

                    _exportExcelService.InitializeWithTemplate(Server.MapPath(DisciplineSummaryTemplatePath));
                    _exportExcelService.ContentRowStartIndex = 4;
                    CreateRedStyleForDisciplineSummaryReport();

                    _exportExcelService.ExportWithTemplate(employeeDisciplines, true, true);
                }
            }

            return new EmptyResult();
        }

        public ActionResult ExportEmployeeFlowSettingReport(GetPeriodEmployeeSettingCriteria criteria)
        {
            var employeeFlowSettings = _periodService.GetPeriodEmployeeSettingById(criteria);

            _exportExcelService.FileName = string.Format(Lang.EmployeeFlowSettingReportName, criteria.PeriodName);
            _exportExcelService.SheetName = Lang.EmployeeFlowSettingReportSheet;
            _exportExcelService.Title = string.Format(Lang.EmployeeFlowSettingReportTitle, criteria.PeriodName);
            _exportExcelService.ContentRowStartIndex = DefaultContentRowStartIndexForTemplate;

            _exportExcelService.InitializeWithTemplate(Server.MapPath(EmployeeFlowSettingTemplate));

            _exportExcelService.ExportWithTemplate(employeeFlowSettings, true, true);

            return new EmptyResult();
        }

        public ActionResult ExportEmployeeInfoReport(GetAllEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                IEnumerable<Employee> employees = _employeeService.GetAllEmployee(criteria);

                if (employees.Any())
                {
                    _exportExcelService.FileName = "Employee_Infomation";
                    _exportExcelService.SheetName = "Employee Infomation";

                    _exportExcelService.InitializeWithTemplate(Server.MapPath(EmployeeBasicInfoTemplate));

                    _exportExcelService.ContentRowStartIndex = DefaultContentRowStartIndexForTemplate;

                    _exportExcelService.ExportWithTemplate(employees, true);
                }
            }

            return new EmptyResult();
        }

        public ActionResult ExportEPAAfterUpdateFinalScore(GetEPAToUpdateFinalScoreCriteria criteria)
        {
            if (criteria != null)
            {
                IList<EPAModel> epaInfos = _employeeEPAService.GetEPAToUpdateFinalScore(criteria);
                string[] employeeIds = criteria.EmployeeIds.Split(',');
                epaInfos = epaInfos.Join(employeeIds, x => x.Employee.EmployeeId.ToString(), y => y, (x, y) => x).ToList();

                if (epaInfos.Any())
                {
                    _exportExcelService.FileName = "Update_Final_Score";
                    _exportExcelService.SheetName = "Update Final Score";
                    _exportExcelService.Title = criteria.PeriodName;

                    _exportExcelService.InitializeWithTemplate(Server.MapPath(UpdateFinalScoreTemplatePath));

                    _exportExcelService.ContentRowStartIndex = DefaultContentRowStartIndexForTemplate;

                    _exportExcelService.ExportWithTemplate(epaInfos, true, true);
                }
            }

            return new EmptyResult();
        }

        public ActionResult ExportEPADetailReport(GetEPADetailCriteria criteria)
        {
            criteria.ActorId = _sessionService.CurrentUser.EmployeeId;

            EPADetailModel epaDetail = _reportService.GetEPADetailReport(criteria);

            epaDetail.Period = _reportService.GetEPADetailPeriodForExportedFileName(criteria.PeriodId?.ToString());

            if (string.IsNullOrEmpty(criteria.SortColumnName) && epaDetail.EpaInfos.Any())
            {
                epaDetail.EpaInfos = epaDetail.EpaInfos.First().Period.TargetTypeId == (int)TargetType.Probation
                    ? epaDetail.EpaInfos.OrderByDescending(x => x.Employee.Code)
                    : epaDetail.EpaInfos.OrderBy(x => x.Employee.GroupCode)
                        .ThenBy(x => x.Employee.Code);
            }

            _epaDetailReportService.Render(epaDetail);

            return new EmptyResult();
        }

        public ActionResult ExportEPASummaryReport(GetEPASummaryCriteria criteria)
        {
            criteria.ActorId = _sessionService.CurrentUser.EmployeeId;

            EPASummaryModel epaSummary = _reportService.GetEPASummaryReport(criteria);
            epaSummary.Period = _reportService.GetEPADetailPeriodForExportedFileName(criteria.PeriodIds);

            MemoryStream output = _reportService.ExportEPASummaryReport(epaSummary);

            string fileName = GetEPASummaryReportFilename(epaSummary);

            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }

        public ActionResult ExportMonthlyEPADetailReport(GetMonthlyEPADetailCriteria criteria)
        {
            criteria.EmployeeId = _sessionService.CurrentUser.EmployeeId;

            EPADetailModel epaDetail = _reportService.GetMonthlyEPADetailReport(criteria);
            _monthlyEPADetailReportService.Render(epaDetail);

            return new EmptyResult();
        }

        public ActionResult ExportMonthlyEPASummaryReport(GetMonthlyEPASummaryCriteria criteria)
        {
            string fileName = Lang.MonthlyEPASummaryReportName + ".xls";

            criteria.EmployeeId = _sessionService.CurrentUser.EmployeeId;

            EPASummaryModel epaSummary = _reportService.GetMonthlyEPASummaryReport(criteria);

            MemoryStream output = _reportService.ExportMonthlyEPASummaryReport(epaSummary);

            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }

        public ActionResult ExportPersonalFlowSettingReport(int flowTemplateId, string flowTemplateName)
        {
            if (flowTemplateId > 0 && !string.IsNullOrEmpty(flowTemplateName))
            {
                IEnumerable<PersonalFlowModel> flowSettings = _personalFlowService.GetPersonalFlowByFlowTemplateId(flowTemplateId);

                _exportExcelService.FileName = "Personal_Flow_Setting";
                _exportExcelService.SheetName = "Personal Flow";
                _exportExcelService.Title = $"{flowTemplateName} - Personal Flow Setting";

                _exportExcelService.InitializeWithTemplate(Server.MapPath(PersonalFlowSettingTemplate));

                _exportExcelService.ContentRowStartIndex = DefaultContentRowStartIndexForTemplate;

                _exportExcelService.ExportWithTemplate(flowSettings, true, true);
            }

            return new EmptyResult();
        }

        public ActionResult ExportTargetPersonnelSettingReport(GetAllTargetEmployeeSettingCriteria criteria)
        {
            if (criteria != null)
            {
                var datas = _targetEmployeeSettingService.GetTargetEmployeeSetting(criteria);

                _exportExcelService.FileName = "Target_Personnel_Setting";
                _exportExcelService.SheetName = "Target Personnel Setting";
                _exportExcelService.Title = "Target Personnel Setting";

                _exportExcelService.InitializeWithTemplate(Server.MapPath(TargetPersonnelSettingTemplate));

                _exportExcelService.ExportWithTemplate(datas, true);
            }

            return new EmptyResult();
        }

        private static ConditionalCellStyle CreateCellStyleIfLessThanZero(int styleIndex, int columnIndex)
        {
            return new ConditionalCellStyle
            {
                ColumnIndex = columnIndex,
                Condition = delegate (object o)
                {
                    var value = o.GetType().GetProperties()[columnIndex].GetValue(o, null);
                    float number;

                    if (!float.TryParse(value.ToString(), out number))
                    {
                        return false;
                    }

                    return number < 0;
                },
                StyleIndex = styleIndex
            };
        }

        private static ConditionalCellStyle CreateCellStyleIfLessThanZeroAndNotWeekend(int styleIndex, int columnIndex)
        {
            return new ConditionalCellStyle
            {
                ColumnIndex = columnIndex,
                Condition = delegate (object o)
                {
                    var value = o.GetType().GetProperties()[7].GetValue(o, null);
                    DateTime dateTime = DateTime.Parse(value.ToString());

                    if (dateTime.DayOfWeek != DayOfWeek.Saturday && dateTime.DayOfWeek != DayOfWeek.Sunday)
                    {
                        value = o.GetType().GetProperties()[columnIndex].GetValue(o, null);
                        return IsFloatLessThanZero(value);
                    }

                    return false;
                },
                StyleIndex = styleIndex
            };
        }

        private static ConditionalCellStyle CreateCellStyleIfLessThanZeroAndWeekend(int styleIndex, int columnIndex)
        {
            return new ConditionalCellStyle
            {
                ColumnIndex = columnIndex,
                Condition = delegate(object o)
                {
                    var value = o.GetType().GetProperties()[7].GetValue(o, null);
                    DateTime dateTime = DateTime.Parse(value.ToString());

                    if (dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday)
                    {
                        value = o.GetType().GetProperties()[columnIndex].GetValue(o, null);
                        return IsFloatLessThanZero(value);
                    }

                    return false;
                },
                StyleIndex = styleIndex
            };
        }

        private static string GetDisciplineDetailReportTitle(GetDisciplineDetailCriteria criteria)
        {
            string reportTitle;
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

            return reportTitle;
        }

        private static string GetDisciplineSummaryByPeriodReportTitle(GetDisciplineSummaryByPeriodCriteria criteria)
        {
            string reportTitle;

            if (criteria.EmployeeId != null)
            {
                reportTitle = $"{criteria.EmployeeName} - Discipline Summary Report";
            }
            else if (criteria.DepartmentId != null)
            {
                reportTitle = $"{criteria.DepartmentName} - Discipline Summary Report";
            }
            else
            {
                reportTitle = "Discipline Summary Report";
            }

            return reportTitle;
        }

        private static string GetDisciplineSummaryReportTitle(GetDisciplineSummaryCriteria criteria)
        {
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

            return reportTitle;
        }

        private static string GetEPASummaryReportFilename(EPASummaryModel epaSummary)
        {
            string fileName = $"{Lang.EPASummaryReportName}.xls";

            if (!string.IsNullOrEmpty(epaSummary.EpaYear) && !string.IsNullOrEmpty(epaSummary.EpaEvaluationPeriod))
            {
                fileName = $"{epaSummary.EpaYear}_{epaSummary.EpaEvaluationPeriod}_{Lang.EPASummaryReportName}.xls";
            }

            return fileName;
        }

        private static bool IsFloatLessThanZero(object value)
        {
            float number;

            if (!float.TryParse(value.ToString(), out number))
            {
                return false;
            }

            return number < 0;
        }

        private void CreateRedStyleForDisciplineSummaryReport()
        {
            // Style
            int redStyleIndex = _exportExcelService.CreateNewColumnStyle();
            ICellStyle style = _exportExcelService.ColumnsStyles[redStyleIndex];
            IFont redFont = _exportExcelService.Workbook.CreateFont();
            redFont.Color = HSSFColor.Red.Index;
            style.SetFont(redFont);

            int columnIndex = 19;
            ConditionalCellStyle substractSystemScoreStyle = CreateCellStyleIfLessThanZero(redStyleIndex, columnIndex);

            _exportExcelService.ConditionalCellStyles.Add(substractSystemScoreStyle);

            columnIndex = 20;

            ConditionalCellStyle approverSubstractScoreStyle = CreateCellStyleIfLessThanZero(redStyleIndex, columnIndex);

            _exportExcelService.ConditionalCellStyles.Add(approverSubstractScoreStyle);
        }

        private void CreateStyleForDisciplineDetailReport(IEnumerable<DisciplineDetailModel> employeeDisciplines)
        {
            int totalColumn = employeeDisciplines.First().ConvertToExcelRecord().GetType().GetProperties().Length;

            // Red font
            IFont redFont = _exportExcelService.Workbook.CreateFont();
            redFont.Color = HSSFColor.Red.Index;

            // Yellow style
            int styleIndex = _exportExcelService.CreateNewColumnStyle();
            ICellStyle style = _exportExcelService.ColumnsStyles[styleIndex];
            style.FillPattern = FillPattern.SolidForeground;
            style.FillBackgroundColor = HSSFColor.LightYellow.Index;
            style.FillForegroundColor = HSSFColor.LightYellow.Index;

            // Yellow with red font style
            int yellowWithRedFontStyleIndex = _exportExcelService.CreateNewColumnStyle();
            ICellStyle yelloWithRedFontStyle = _exportExcelService.ColumnsStyles[yellowWithRedFontStyleIndex];
            yelloWithRedFontStyle.FillPattern = FillPattern.SolidForeground;
            yelloWithRedFontStyle.FillBackgroundColor = HSSFColor.LightYellow.Index;
            yelloWithRedFontStyle.FillForegroundColor = HSSFColor.LightYellow.Index;
            yelloWithRedFontStyle.SetFont(redFont);

            // Normal with red font style
            int normalWithRedFontStyleIndex = _exportExcelService.CreateNewColumnStyle();
            ICellStyle normalWithRedFontStyle = _exportExcelService.ColumnsStyles[normalWithRedFontStyleIndex];
            normalWithRedFontStyle.SetFont(redFont);

            // Condition

            // Yellow
            for (int i = 0; i < totalColumn; i++)
            {
                ConditionalCellStyle yellowCellStyle = new ConditionalCellStyle
                {
                    ColumnIndex = i,
                    Condition = delegate(object o)
                    {
                        var value = o.GetType().GetProperties()[7].GetValue(o, null);
                        DateTime dateTime = DateTime.Parse(value.ToString());

                        return dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;
                    },
                    StyleIndex = styleIndex
                };
                _exportExcelService.ConditionalCellStyles.Add(yellowCellStyle);
            }

            ConditionalCellStyle weekendWithRedScoringSystem =
                CreateCellStyleIfLessThanZeroAndWeekend(yellowWithRedFontStyleIndex, 22);
            _exportExcelService.ConditionalCellStyles.Add(weekendWithRedScoringSystem);

            ConditionalCellStyle weekendWithRedScoringApprover =
                CreateCellStyleIfLessThanZeroAndWeekend(yellowWithRedFontStyleIndex, 23);
            _exportExcelService.ConditionalCellStyles.Add(weekendWithRedScoringApprover);

            // Normal
            ConditionalCellStyle normalWithRedScoringSystem =
                CreateCellStyleIfLessThanZeroAndNotWeekend(normalWithRedFontStyleIndex, 22);
            _exportExcelService.ConditionalCellStyles.Add(normalWithRedScoringSystem);

            ConditionalCellStyle normalWithRedScoringApprover =
                CreateCellStyleIfLessThanZeroAndNotWeekend(normalWithRedFontStyleIndex, 23);
            _exportExcelService.ConditionalCellStyles.Add(normalWithRedScoringApprover);
        }
    }
}