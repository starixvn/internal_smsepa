namespace SMS.EPA.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using GO.UserControl.Client.Core.Models.Role;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories;
    using SMS.EPA.Resources;

#pragma warning disable S109 // Magic numbers should not be used
    public class ReportService : IReportService
    {
        private const int ExcelColumnWidth = 256;
        private readonly IEmployeeEPARepository employeeEPARepository;
        private readonly IEmployeeEPAService employeeEPAService;
        private readonly IReportRepository reportRepository;
        private readonly IBaseService baseService;

        public ReportService() : this(null, null, null, null)
        {
        }

        public ReportService(
            IReportRepository reportRepository,
            IEmployeeEPARepository employeeEPARepository,
            IEmployeeEPAService employeeEPAService,
            IBaseService baseService)
        {
            this.reportRepository = reportRepository ?? new ReportRepository();
            this.employeeEPARepository = employeeEPARepository ?? new EmployeeEPARepository();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.baseService = baseService ?? new BaseService();
        }

        public MemoryStream ExportEPASummaryReport(EPASummaryModel epaSummary)
        {
            return ExportEPASummaryReport(epaSummary, Lang.EPASummaryReportTitle);
        }

        public MemoryStream ExportMonthlyEPASummaryReport(EPASummaryModel epaSummary)
        {
            return ExportEPASummaryReport(epaSummary, Lang.MonthlyEPASummaryReportTitle);
        }

        public DashboardModel GetDashboard(int employeeId)
        {
            var criteria = new GetDashboardCriteria
            {
                EmployeeId = employeeId
            };

            DashboardModel result = employeeEPARepository.GetDashboard(criteria);
            result.MonthlyEpaInfos = result.MyEpaInfos.Where(x => x.Period.TargetTypeId == (byte)TargetType.Monthly);
            result.MyEpaInfos = result.MyEpaInfos.Where(x => x.Period.TargetTypeId != (byte)TargetType.Monthly);

            return result;
        }

        public IEnumerable<EmployeeViewModel> GetEmployeeByActorId(GetEmployeeByActorIdCriteria criteria)
        {
            return baseService.GetData<EmployeeViewModel>(criteria);
        }

        public PeriodModel GetEPADetailPeriodForExportedFileName(string periodId)
        {
            int id;
            if (!string.IsNullOrEmpty(periodId) && int.TryParse(periodId, out id))
            {
                var criteria = new GetPeriodByIdCriteria
                {
                    PeriodId = id
                };

                return baseService.GetDataEntity<PeriodModel>(criteria);
            }

            return null;
        }

        public EPADetailModel GetEPADetailReport(GetEPADetailCriteria criteria, int currentEmployeeId, IList<Role> roles)
        {
            EPADetailModel result = reportRepository.GetEPADetail(criteria);
            foreach (EPAModel epaInfo in result.EpaInfos)
            {
                employeeEPAService.RemoveSensitiveDataOnEmployeeRole(epaInfo, currentEmployeeId, roles);
            }

            string epaIds = string.Join(",", result.EpaInfos.Where(x => x.IsSemiHasMonthly && x.EPAId > 0).Select(x => x.EPAId));
            if (!string.IsNullOrEmpty(epaIds))
            {
                IList<EPAModel> monthlyEPAs = employeeEPAService.GetMonthlyEPAOfSemi(epaIds);
                foreach (EPAModel epaInfo in result.EpaInfos)
                {
                    employeeEPAService.GroupMonthlyEPABySemi(epaInfo, monthlyEPAs);
                    employeeEPAService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }
            }

            return result;
        }

        public EPASummaryModel GetEPASummaryReport(GetEPASummaryCriteria criteria)
        {
            var standardGrades = baseService.GetData<StandardGradeModel>(new GetStandardGradeCriteria());
            EPASummaryModel result = reportRepository.GetEPASummary(criteria);

            var epaIds = new StringBuilder();
            foreach (EPASummaryEmployeeModel epaInfo in result.EPAInfos)
            {
                epaIds.AppendFormat("{0},", string.Join(",", epaInfo.Periods.Where(x => x.IsSemiHasMonthly && x.EPAId > 0).Select(x => x.EPAId)));
            }

            IList<EPAModel> monthlyEPAs = epaIds.Length > 0 ? employeeEPAService.GetMonthlyEPAOfSemi(epaIds.ToString()) : new List<EPAModel>();
            foreach (EPASummaryEmployeeModel epaInfo in result.EPAInfos)
            {
                foreach (EPASummaryPeriodModel epaInfoPeriod in epaInfo.Periods)
                {
                    float? performanceScore = GetPerformanceScore(epaInfoPeriod);

                    if (epaInfoPeriod.IsSemiHasMonthly && epaInfoPeriod.EPAId > 0)
                    {
                        epaInfoPeriod.MonthlyEPA.MonthlyEPAs = monthlyEPAs.Where(x => x.EPAParentId.Equals(epaInfoPeriod.EPAId)).ToList();
                        employeeEPAService.ConvertAverageMonthlyScore(epaInfoPeriod.MonthlyEPA, epaInfoPeriod.ScoreRatingType);

                        performanceScore = GetPerformanceScoreForSemiHasMonthly(epaInfoPeriod);
                    }

                    if (performanceScore.HasValue)
                    {
                        epaInfoPeriod.FinalScore = (performanceScore.Value + epaInfoPeriod.DisciplineScore).ToString(CultureInfo.InvariantCulture);
                        epaInfoPeriod.Grade = employeeEPAService.ConvertScoreToGrade(
                            performanceScore,
                            epaInfoPeriod.DisciplineScore,
                            epaInfoPeriod.HasDisciplineScore,
                            standardGrades);
                    }
                }
            }

            return result;
        }

        public EPADetailModel GetMonthlyEPADetailReport(GetMonthlyEPADetailCriteria criteria, int currentEmployeeId, IList<Role> roles)
        {
            EPADetailModel result = reportRepository.GetMonthlyEPADetail(criteria);
            foreach (EPAModel epaInfo in result.EpaInfos)
            {
                employeeEPAService.RemoveSensitiveDataOnEmployeeRole(epaInfo, currentEmployeeId, roles);
            }

            return result;
        }

        public EPASummaryModel GetMonthlyEPASummaryReport(GetMonthlyEPASummaryCriteria criteria)
        {
            return reportRepository.GetMonthlyEPASummary(criteria);
        }

        private static void CreateBodyStyles(
            HSSFWorkbook workbook,
            out ICellStyle bodyStyle1,
            out ICellStyle bodyStyle2,
            out ICellStyle bodyStyle3,
            out ICellStyle bodyStyle4)
        {
            bodyStyle1 = workbook.CreateCellStyle();
            bodyStyle1.Alignment = HorizontalAlignment.Center;
            bodyStyle1.VerticalAlignment = VerticalAlignment.Center;

            bodyStyle2 = workbook.CreateCellStyle();
            bodyStyle2.Alignment = HorizontalAlignment.Center;
            bodyStyle2.VerticalAlignment = VerticalAlignment.Center;
            bodyStyle2.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            bodyStyle2.FillPattern = FillPattern.SolidForeground;
            bodyStyle2.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;

            bodyStyle3 = workbook.CreateCellStyle();
            bodyStyle3.Alignment = HorizontalAlignment.Left;

            bodyStyle4 = workbook.CreateCellStyle();
            bodyStyle4.Alignment = HorizontalAlignment.Left;
            bodyStyle4.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            bodyStyle4.FillPattern = FillPattern.SolidForeground;
            bodyStyle4.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;
        }

#pragma warning disable S107 // Methods should not have too many parameters
        private static ISheet CreateCommonCells(
            ISheet sheet,
            int rowIndex,
            ICellStyle bodyStyle1,
            ICellStyle bodyStyle2,
            ICellStyle bodyStyle3,
            ICellStyle bodyStyle4,
            EPASummaryEmployeeModel epaInfo,
            out ICellStyle bodyStyle,
            out IRow row,
            out ICell cellBody)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            bodyStyle = rowIndex % 2 == 0 ? bodyStyle2 : bodyStyle1;
            row = sheet.CreateRow(rowIndex);

            cellBody = row.CreateCell(0);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(rowIndex - 3);
            SetColumnWidth(ref sheet, 0, 5);

            cellBody = row.CreateCell(1);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(epaInfo.Code);
            SetColumnWidth(ref sheet, 1, 10);

            bodyStyle = rowIndex % 2 == 0 ? bodyStyle2 : bodyStyle1;
            cellBody = row.CreateCell(2);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(epaInfo.NickName);
            SetColumnWidth(ref sheet, 2, 12);

            bodyStyle = rowIndex % 2 == 0 ? bodyStyle4 : bodyStyle3;
            cellBody = row.CreateCell(3);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(epaInfo.FullName);
            SetColumnWidth(ref sheet, 3, 20);

            bodyStyle = rowIndex % 2 == 0 ? bodyStyle4 : bodyStyle3;
            cellBody = row.CreateCell(4);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(epaInfo.Category);
            SetColumnWidth(ref sheet, 4, 12);

            cellBody = row.CreateCell(5);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(epaInfo.Department);
            SetColumnWidth(ref sheet, 5, 16);

            cellBody = row.CreateCell(6);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(epaInfo.Section);
            SetColumnWidth(ref sheet, 6, 16);

            cellBody = row.CreateCell(7);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(epaInfo.Group.GroupName);
            SetColumnWidth(ref sheet, 7, 16);

            cellBody = row.CreateCell(8);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(epaInfo.JobTitle.JobTitleName);
            SetColumnWidth(ref sheet, 8, 25);

            bodyStyle = rowIndex % 2 == 0 ? bodyStyle2 : bodyStyle1;
            cellBody = row.CreateCell(9);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(epaInfo.JoinedDate.ToString(Constants.DateFormat));
            SetColumnWidth(ref sheet, 9, 15);

            return sheet;
        }

        private static MemoryStream ExportEPASummaryReport(EPASummaryModel epaSummary, string title)
        {
            const short fontHeight = 18;
            const int tableHeaderRowIndex = 3;

            var titleHeaders = new List<string>
            {
                Lang.SymbolCount,
                Lang.Code,
                Lang.NickName,
                Lang.FullName,
                Lang.Category,
                Lang.Department,
                Lang.Section,
                Lang.Team,
                Lang.JobTitle,
                Lang.GroupDate
            };

            var workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(Lang.EPASummaryReportSheet);
            IRow rowTitle = sheet.CreateRow(0);

            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = fontHeight;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICell cell = rowTitle.CreateCell(0);
            cell.SetCellValue(title);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Center;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle.SetFont(font);

            int lastColumn = titleHeaders.Count + (epaSummary.PeriodSelecteds.Count * 2) - 1;

            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, lastColumn));

            RenderHeaderEPASummaryReport(epaSummary.PeriodSelecteds, ref workbook, ref sheet, titleHeaders);
            RenderContentRowEPASummaryReport(epaSummary, ref workbook, ref sheet);
            sheet.CreateFreezePane(0, tableHeaderRowIndex + 1);

            MemoryStream output = null;
            try
            {
                output = new MemoryStream();
                workbook.Write(output);

                return output;
            }
            finally
            {
                output?.Dispose();
            }
        }

        private static void GenerateFinalScoreAndGradeCell(
            ref ISheet sheet,
            ICellStyle bodyStyle,
            IRow row,
            int i,
            string finalScore,
            string grade,
            IFont redFont = null)
        {
            int summaryColIndex = 10 + (2 * i);

            ICell cellBody = row.CreateCell(summaryColIndex);
            cellBody.CellStyle = bodyStyle;
            if (redFont != null)
            {
                cellBody.CellStyle.SetFont(redFont);
            }

            cellBody.SetCellValue(finalScore);
            SetColumnWidth(ref sheet, summaryColIndex, 14);

            cellBody = row.CreateCell(summaryColIndex + 1);
            cellBody.CellStyle = bodyStyle;
            if (redFont != null)
            {
                cellBody.CellStyle.SetFont(redFont);
            }

            cellBody.SetCellValue(grade);
            SetColumnWidth(ref sheet, summaryColIndex + 1, 10);
        }

        private static string GetFinalScoreString(EPASummaryPeriodModel period)
        {
            const int numOfFractionalDigits = 2;
            string finalScore = string.Empty;
            if (period != null)
            {
                finalScore = string.IsNullOrEmpty(period.FinalScore)
                    ? Lang.NotAvailable
                    : Math.Round(float.Parse(period.FinalScore), numOfFractionalDigits).ToString(CultureInfo.InvariantCulture);
            }

            return finalScore;
        }

        private static string GetGradeString(EPASummaryPeriodModel period)
        {
            string grade = string.Empty;
            if (period != null)
            {
                grade = string.IsNullOrEmpty(period.FinalScore) ? Lang.NotAvailable : period.Grade;
            }

            return grade;
        }

        private static void RenderContentRowEPASummaryReport(
            EPASummaryModel epaSummary,
            ref HSSFWorkbook workbook,
            ref ISheet sheet)
        {
            var rowIndex = 4;

            ICellStyle bodyStyle1;
            ICellStyle bodyStyle2;
            ICellStyle bodyStyle3;
            ICellStyle bodyStyle4;

            CreateBodyStyles(
                workbook,
                out bodyStyle1,
                out bodyStyle2,
                out bodyStyle3,
                out bodyStyle4);

            foreach (var epaInfo in epaSummary.EPAInfos)
            {
                ICellStyle bodyStyle;
                IRow row;
                ICell cellBody;

                sheet = CreateCommonCells(
                    sheet,
                    rowIndex,
                    bodyStyle1,
                    bodyStyle2,
                    bodyStyle3,
                    bodyStyle4,
                    epaInfo,
                    out bodyStyle,
                    out row,
                    out cellBody);

                List<int> periodIds = epaInfo.Periods.Select(x => x.PeriodId).ToList();
                for (int i = 0; i < epaSummary.PeriodSelecteds.Count; i++)
                {
                    if (periodIds.Contains(epaSummary.PeriodSelecteds[i].PeriodId))
                    {
                        EPASummaryPeriodModel period =
                            epaInfo.Periods.FirstOrDefault(x => x.PeriodId == epaSummary.PeriodSelecteds[i].PeriodId);

                        string finalScore = GetFinalScoreString(period);
                        string grade = GetGradeString(period);

                        IFont redFont = workbook.CreateFont();
                        redFont.Color = HSSFColor.Red.Index;

                        ICellStyle newStyle = workbook.CreateCellStyle();
                        newStyle.CloneStyleFrom(bodyStyle);

                        GenerateFinalScoreAndGradeCell(
                            ref sheet,
                            newStyle,
                            row,
                            i,
                            finalScore,
                            grade,
                            period != null && period.StatusId == (byte)EpaStatus.Open ? redFont : null);
                    }
                    else
                    {
                        int summaryColumnIndex = 10 + (2 * i);

                        cellBody = row.CreateCell(summaryColumnIndex);
                        cellBody.CellStyle = bodyStyle;
                        cellBody.SetCellValue("-");
                        SetColumnWidth(ref sheet, summaryColumnIndex, 14);

                        cellBody = row.CreateCell(summaryColumnIndex + 1);
                        cellBody.CellStyle = bodyStyle;
                        cellBody.SetCellValue("-");
                        SetColumnWidth(ref sheet, summaryColumnIndex, 10);
                    }
                }

                rowIndex++;
            }
        }

        private static void RenderHeaderEPASummaryReport(
            IList<PeriodModel> periodSelecteds,
            ref HSSFWorkbook workbook,
            ref ISheet sheet,
            IList<string> titleHeaders)
        {
            const int fontHeight = 11;

            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = fontHeight;
            font.Color = HSSFColor.White.Index;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillBackgroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.SetFont(font);

            IRow row = sheet.CreateRow(2);
            for (int i = 0; i < titleHeaders.Count; i++)
            {
                var cell = row.CreateCell(i);
                cell.CellStyle = cellStyle;
                cell.SetCellValue(titleHeaders[i]);

                sheet.AddMergedRegion(new CellRangeAddress(2, 3, i, i));
            }

            for (int i = 0; i < periodSelecteds.Count; i++)
            {
                var cell = row.CreateCell(titleHeaders.Count + (2 * i));
                cell.CellStyle = cellStyle;
                cell.SetCellValue(periodSelecteds[i].Name);

                sheet.AddMergedRegion(new CellRangeAddress(
                    2,
                    2,
                    titleHeaders.Count + (2 * i),
                    titleHeaders.Count + ((2 * i) + 1)));
            }

            IRow rowPeriod = sheet.CreateRow(3);
            for (int i = 0; i < periodSelecteds.Count; i++)
            {
                var cell = rowPeriod.CreateCell(titleHeaders.Count + (2 * i));
                cell.CellStyle = cellStyle;
                cell.SetCellValue(Lang.FinalScore);

                cell = rowPeriod.CreateCell(titleHeaders.Count + ((2 * i) + 1));
                cell.CellStyle = cellStyle;
                cell.SetCellValue(Lang.Grade);
            }
        }

        private static void SetColumnWidth(ref ISheet sheet, int columnIndex, int width)
        {
            sheet.SetColumnWidth(columnIndex, width * ExcelColumnWidth);
        }

        private static float? GetPerformanceScore(EPASummaryPeriodModel epaInfo)
        {
            float? performanceScore = null;
            var detailScores = epaInfo.DetailScores.Where(x => !string.IsNullOrEmpty(x.TotalScore)
                                                    && x.ProcessId > (byte)ProcessType.Self
                                                    && x.StatusId > (byte)ProcessStatusType.Open);

            if (detailScores.Any())
            {
                var maxProcessId = detailScores.Max(x => x.ProcessId);
                var lastPhaseScores = epaInfo.DetailScores.Where(x => !string.IsNullOrEmpty(x.TotalScore) && x.ProcessId == maxProcessId).Select(x => x.TotalScoreConverted);
                performanceScore = lastPhaseScores.Any() ? (float)Math.Round(lastPhaseScores.Average(), Constants.NumberOfDecimalDefault) : performanceScore;
            }

            return performanceScore;
        }

        private static float? GetPerformanceScoreForSemiHasMonthly(EPASummaryPeriodModel epaInfo)
        {
            float? performanceScore = null;
            var detailScores = epaInfo.DetailScores.Where(x => !string.IsNullOrEmpty(x.TotalScore)
                                                    && x.ProcessId > (byte)ProcessType.Evaluator
                                                    && x.StatusId > (byte)ProcessStatusType.Open);

            if (detailScores.Any())
            {
                var maxProcessId = detailScores.Max(x => x.ProcessId);
                IEnumerable<float> lastPhaseScores;
                switch (maxProcessId)
                {
                    case (byte)ProcessType.Reviewer:
                        lastPhaseScores = epaInfo.DetailScores.Where(x => !string.IsNullOrEmpty(x.TotalScore) && x.ProcessId == maxProcessId).Select(x => x.TotalScoreConverted);
                        performanceScore = lastPhaseScores.Any()
                            ? (float)Math.Round(lastPhaseScores.Average(), Constants.NumberOfDecimalDefault)
                            : epaInfo.MonthlyEPA.AverageReviewerScoreConverted;
                        break;

                    case (byte)ProcessType.Management:
                        lastPhaseScores = epaInfo.DetailScores.Where(x => !string.IsNullOrEmpty(x.TotalScore) && x.ProcessId == maxProcessId).Select(x => x.TotalScoreConverted);
                        performanceScore = lastPhaseScores.Any() ? (float)Math.Round(lastPhaseScores.Average(), Constants.NumberOfDecimalDefault) : performanceScore;
                        break;

                    default:
                        performanceScore = null;
                        break;
                }
            }

            return performanceScore;
        }
    }
#pragma warning restore S109 // Magic numbers should not be used
}