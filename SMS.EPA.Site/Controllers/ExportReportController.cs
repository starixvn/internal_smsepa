namespace SMS.EPA.Site.Controllers
{
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
        private const string DisciplineSummaryTemplatePath = @"~\ExcelTemplates\DisciplineSummaryReport.xls";
        private const string DisciplineDetailTemplatePath = @"~\ExcelTemplates\DisciplineDetailReport.xls";
        private const string DisciplineSummaryByPeriodTemplatePath = @"~\ExcelTemplates\DisciplineSummaryByPeriodReport.xls";
        private const string UpdateFinalScoreTemplatePath = @"~\ExcelTemplates\UpdateFinalScore.xls";
        private const string EmployeeFlowSettingTemplate = @"~\ExcelTemplates\EmployeeFlowSettingTemplate.xls";

        private const int DefaultContentRowStartIndexForTemplate = 3;

        private readonly IReportService _reportService;
        private readonly IEPADetailReportService _epaDetailReportService;
        private readonly IMonthlyEPADetailReportService _monthlyEPADetailReportService;
        private readonly IDisciplineService _disciplineService;
        private readonly IEmployeeEPAService _employeeEPAService;
        private readonly IExportEmployeeInfoService _exportEmployeeInfoService;
        private readonly IExportPersonalFlowSettingService _exportPersonalFlowSettingService;
        private readonly IExportExcelService _exportExcelService;
        private readonly ITargetEmployeeSettingService _targetEmployeeSettingService;
        private readonly IPeriodService _periodService;
        private readonly ISessionService _sessionService;

        public ExportReportController()
            : this(null, null, null, null, null, null, null, null, null, null, null)
        {
        }

        public ExportReportController(
            IReportService reportService,
            IEPADetailReportService epaDetailReportService,
            IMonthlyEPADetailReportService monthlyEPADetailReportService,
            IDisciplineService disciplineService,
            IEmployeeEPAService employeeEPAService,
            IExportEmployeeInfoService exportEmployeeInfoService,
            IExportPersonalFlowSettingService exportPersonalFlowSettingService,
            IExportExcelService exportExcelService,
            ITargetEmployeeSettingService targetEmployeeSettingService,
            IPeriodService periodService,
            ISessionService sessionService)
        {
            _reportService = reportService ?? new ReportService();
            _epaDetailReportService = epaDetailReportService ?? new EPADetailReportService();
            _monthlyEPADetailReportService = monthlyEPADetailReportService ?? new MonthlyEPADetailReportService();
            _disciplineService = disciplineService ?? new DisciplineService();
            _sessionService = sessionService ?? new SessionService();
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            _exportEmployeeInfoService = exportEmployeeInfoService ?? new ExportEmployeeInfoService();
            _exportPersonalFlowSettingService = exportPersonalFlowSettingService ?? new ExportPersonalFlowSettingService();
            _exportExcelService = exportExcelService ?? new ExportExcelService();
            _targetEmployeeSettingService = targetEmployeeSettingService ?? new TargetEmployeeSettingService();
            _periodService = periodService ?? new PeriodService();
        }

        public ActionResult ExportEmployeeFlowSettingReport(GetPeriodEmployeeSettingCriteria criteria)
        {
            var employeeFlowSettings = _periodService.GetPeriodEmployeeSettingById(criteria);

            _exportExcelService.FileName = string.Format(Lang.EmployeeFlowSettingReportName, criteria.PeriodName);
            _exportExcelService.SheetName = Lang.EmployeeFlowSettingReportSheet;
            _exportExcelService.Title = string.Format(Lang.EmployeeFlowSettingReportTitle, criteria.PeriodName);
            _exportExcelService.ContentRowStartIndex = DefaultContentRowStartIndexForTemplate;

            _exportExcelService.InitializeWithTemplate(Server.MapPath(EmployeeFlowSettingTemplate));

            _exportExcelService.ExportWithTemplate(employeeFlowSettings, true);

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

        private string GetEPASummaryReportFilename(EPASummaryModel epaSummary)
        {
            string fileName = $"{Lang.EPASummaryReportName}.xls";

            if (!string.IsNullOrEmpty(epaSummary.EpaYear) && !string.IsNullOrEmpty(epaSummary.EpaEvaluationPeriod))
            {
                fileName = $"{epaSummary.EpaYear}_{epaSummary.EpaEvaluationPeriod}_{Lang.EPASummaryReportName}.xls";
            }

            return fileName;
        }

        public ActionResult ExportMonthlyEPASummaryReport(GetMonthlyEPASummaryCriteria criteria)
        {
            string fileName = Lang.MonthlyEPASummaryReportName + ".xls";

            criteria.EmployeeId = _sessionService.CurrentUser.EmployeeId;

            EPASummaryModel epaSummary = _reportService.GetMonthlyEPASummaryReport(criteria);

            MemoryStream output = _reportService.ExportMonthlyEPASummaryReport(epaSummary);

            return File(output.ToArray(), "application/vnd.ms-excel", fileName);
        }

        public ActionResult ExportEPADetailReport(GetEPADetailCriteria criteria)
        {
            criteria.ActorId = _sessionService.CurrentUser.EmployeeId;

            EPADetailModel epaDetail = _reportService.GetEPADetailReport(criteria);

            epaDetail.Period = _reportService.GetEPADetailPeriodForExportedFileName(criteria.PeriodId?.ToString());

            if (string.IsNullOrEmpty(criteria.SortColumnName) && epaDetail.EpaInfos.Any())
            {
                switch (epaDetail.EpaInfos.First().Period.TargetTypeId)
                {
                    case (int)TargetType.Probation:
                        epaDetail.EpaInfos = epaDetail.EpaInfos.OrderByDescending(x => x.Employee.Code);
                        break;

                    default:
                        epaDetail.EpaInfos = epaDetail.EpaInfos.OrderBy(x => x.Employee.GroupCode).ThenBy(x => x.Employee.Code);
                        break;
                }
            }

            _epaDetailReportService.Render(epaDetail);

            return new EmptyResult();
        }

        public ActionResult ExportMonthlyEPADetailReport(GetMonthlyEPADetailCriteria criteria)
        {
            criteria.EmployeeId = _sessionService.CurrentUser.EmployeeId;

            EPADetailModel epaDetail = _reportService.GetMonthlyEPADetailReport(criteria);
            _monthlyEPADetailReportService.Render(epaDetail);

            return new EmptyResult();
        }

        private string GetDisciplineSummaryReportTitle(GetDisciplineSummaryCriteria criteria)
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

                    _exportExcelService.ExportWithTemplate(employeeDisciplines, true);
                }
            }

            return new EmptyResult();
        }

        private string GetDisciplineDetailReportTitle(GetDisciplineDetailCriteria criteria)
        {
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

            return reportTitle;
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

                    _exportExcelService.ExportWithTemplate(employeeDisciplines, true);
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
                    string templatePath = Server.MapPath(DisciplineSummaryByPeriodTemplatePath);
                    _disciplineService.ExportDisciplineSummaryByPeriodReport(criteria, employeeDisciplines, templatePath);
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

                if (epaInfos != null && epaInfos.Any())
                {
                    string templatePath = Server.MapPath(UpdateFinalScoreTemplatePath);
                    _employeeEPAService.ExportEPAAfterUpdateFinalScore(criteria.PeriodName, epaInfos, templatePath);
                }
            }

            return new EmptyResult();
        }

        public ActionResult ExportEmployeeInfoReport(GetAllEmployeeCriteria criteria)
        {
            if (criteria != null)
            {
                _exportEmployeeInfoService.Export(criteria);
            }

            return new EmptyResult();
        }

        public ActionResult ExportPersonalFlowSettingReport(int flowTemplateId, string flowTemplateName)
        {
            if (flowTemplateId > 0 && !string.IsNullOrEmpty(flowTemplateName))
            {
                _exportPersonalFlowSettingService.Export(flowTemplateId, flowTemplateName);
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
                _exportExcelService.Initialize();

                //REVIEW: add format to a column base on condition
                //int index = _exportExcelService.CreateNewColumnStyle();
                ////Set Cell Style
                //ICellStyle style = _exportExcelService.ColumnsStyles[index];
                //style.FillBackgroundColor = HSSFColor.Red.Index;
                //style.FillForegroundColor = HSSFColor.Red.Index;
                //style.FillPattern = FillPattern.SolidForeground;
                //style.Alignment = HorizontalAlignment.Justify;

                //ConditionalCellStyle conditionalCellStyle = new ConditionalCellStyle
                //{
                //    ColumnIndex = 0,
                //    StyleIndex = index,
                //    Condition = (x) => x.ToString().Length > 4
                //};
                //_exportExcelService.ConditionalCellStyles.Add(conditionalCellStyle);

                _exportExcelService.Export(datas);
            }

            return new EmptyResult();
        }
    }
}