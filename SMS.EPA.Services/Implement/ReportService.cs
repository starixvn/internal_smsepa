namespace SMS.EPA.Services.Implement
{
    using Core.Constants;
    using Core.Enumerations;
    using Core.Services;
    using Criteria;
    using Criteria.Report;
    using Interfaces;
    using Model;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using Repositories;
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    public class ReportService : BaseService, IReportService
    {
        private const int ExcelColumnWidth = 256;
        private readonly IReportRepository _reportRepository;
        private readonly IEmployeeEPARepository _employeeEPARepository;
        private readonly IEmployeeEPAService _employeeEPAService;

        public ReportService() : this(null, null, null)
        {
        }

        public ReportService(
            IReportRepository reportRepository,
            IEmployeeEPARepository employeeEPARepository,
            IEmployeeEPAService employeeEPAService)
        {
            _reportRepository = reportRepository ?? new ReportRepository();
            _employeeEPARepository = employeeEPARepository ?? new EmployeeEPARepository();
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
        }

        public DashboardModel GetDashboard(int employeeId)
        {
            var criteria = new GetDashboardCriteria
            {
                EmployeeId = employeeId
            };
            var result = new DashboardModel();

            if (criteria.IsValid())
            {
                result = _employeeEPARepository.GetDashboard(criteria);
                result.MonthlyEpaInfos = result.MyEpaInfos.Where(x => x.Period.TargetTypeId == (int)TargetType.Monthly);
                result.MyEpaInfos = result.MyEpaInfos.Where(x => x.Period.TargetTypeId != (int)TargetType.Monthly);
            }

            return result;
        }

        public EPASummaryModel GetEPASummaryReport(GetEPASummaryCriteria criteria)
        {
            EPASummaryModel result = new EPASummaryModel();

            if (criteria != null && criteria.IsValid())
            {
                result = _reportRepository.GetEPASummary(criteria);
            }

            return result;
        }

        public MemoryStream ExportEPASummaryReport(EPASummaryModel epaSummary)
        {
            return ExportEPASummaryReport(epaSummary, Lang.EPASummaryReportTitle);
        }

        public EPADetailModel GetEPADetailReport(GetEPADetailCriteria criteria)
        {
            EPADetailModel result = new EPADetailModel();

            if (criteria != null && criteria.IsValid())
            {
                result = _reportRepository.GetEPADetail(criteria);

                IList<EPAModel> monthlyEPAs = new List<EPAModel>();
                string epaIds = string.Join(",", result.EpaInfos.Where(x => x.IsSemiHasMonthly).Select(x => x.EPAId));

                if (!string.IsNullOrEmpty(epaIds))
                {
                    monthlyEPAs = _employeeEPAService.GetMonthlyEPAOfSemi(epaIds);
                }

                foreach (EPAModel epaInfo in result.EpaInfos)
                {
                    _employeeEPAService.GroupMonthlyEPABySemi(epaInfo, monthlyEPAs);
                    _employeeEPAService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }
            }

            return result;
        }

        public PeriodModel GetEPADetailPeriodForExportedFileName(string periodId)
        {
            int id;

            if (!string.IsNullOrEmpty(periodId) && int.TryParse(periodId, out id))
            {
                GetPeriodByIdCriteria getPeriodByIdCriteria =
                    new GetPeriodByIdCriteria
                    {
                        PeriodId = id
                    };

                return GetDataEntity<PeriodModel>(getPeriodByIdCriteria);
            }

            return null;
        }

        public MemoryStream ExportMonthlyEPASummaryReport(EPASummaryModel epaSummary)
        {
            return ExportEPASummaryReport(epaSummary, Lang.MonthlyEPASummaryReportTitle);
        }

        public EPADetailModel GetMonthlyEPADetailReport(GetMonthlyEPADetailCriteria criteria)
        {
            EPADetailModel result = new EPADetailModel();

            if (criteria != null && criteria.IsValid())
            {
                result = _reportRepository.GetMonthlyEPADetail(criteria);
            }

            return result;
        }

        public EPASummaryModel GetMonthlyEPASummaryReport(GetMonthlyEPASummaryCriteria criteria)
        {
            EPASummaryModel result = new EPASummaryModel();

            if (criteria != null && criteria.IsValid())
            {
                result = _reportRepository.GetMonthlyEPASummary(criteria);
            }

            return result;
        }

        public IEnumerable<EmployeeViewModel> GetEmployeeByActorId(GetEmployeeByActorIdCriteria criteria)
        {
            IEnumerable<EmployeeViewModel> employees = new List<EmployeeViewModel>();

            if (criteria != null && criteria.IsValid())
            {
                employees = GetData<EmployeeViewModel>(criteria);
            }

            return employees;
        }

        private static MemoryStream ExportEPASummaryReport(EPASummaryModel epaSummary, string title)
        {
            IList<string> titleHeaders = new List<string>
            {
                Lang.SymbolCount,
                Lang.Code,
                Lang.FullName,
                Lang.NickName,
                Lang.Category,
                Lang.Section,
                Lang.JobTitle,
                Lang.GroupDate
            };

            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(Lang.EPASummaryReportSheet);
            IRow rowTitle = sheet.CreateRow(0);

            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICell cell = rowTitle.CreateCell(0);
            cell.SetCellValue(title);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Center;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle.SetFont(font);

            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0,
                titleHeaders.Count + (epaSummary.PeriodSelecteds.Count * 2) - 1));

            RenderHeaderEPASummaryReport(epaSummary.PeriodSelecteds, ref workbook, ref sheet, titleHeaders);
            RenderContentRowEPASummaryReport(epaSummary, ref workbook, ref sheet);
            sheet.CreateFreezePane(0, 4);

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

        private static void RenderHeaderEPASummaryReport(
            IList<PeriodModel> periodSelecteds,
            ref HSSFWorkbook workbook,
            ref ISheet sheet,
            IList<string> titleHeaders)
        {
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = 11;
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

                sheet.AddMergedRegion(new CellRangeAddress(2, 2, titleHeaders.Count + (2 * i),
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

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
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

            CreateBodyStyles(workbook, out bodyStyle1, out bodyStyle2, out bodyStyle3, out bodyStyle4);

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

                        cellBody = row.CreateCell(8 + (2 * i));
                        cellBody.CellStyle = bodyStyle;
                        cellBody.SetCellValue(finalScore);
                        SetColumnWidth(ref sheet, 8 + (2 * i), 14);

                        cellBody = row.CreateCell(8 + ((2 * i) + 1));
                        cellBody.CellStyle = bodyStyle;
                        cellBody.SetCellValue(grade);
                        SetColumnWidth(ref sheet, 8 + ((2 * i) + 1), 10);
                    }
                    else
                    {
                        cellBody = row.CreateCell(8 + (2 * i));
                        cellBody.CellStyle = bodyStyle;
                        cellBody.SetCellValue("-");
                        SetColumnWidth(ref sheet, 8 + (2 * i), 14);

                        cellBody = row.CreateCell(8 + ((2 * i) + 1));
                        cellBody.CellStyle = bodyStyle;
                        cellBody.SetCellValue("-");
                        SetColumnWidth(ref sheet, 8 + ((2 * i) + 1), 10);
                    }
                }

                rowIndex++;
            }
        }

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

            bodyStyle = rowIndex % 2 == 0 ? bodyStyle4 : bodyStyle3;

            cellBody = row.CreateCell(2);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(epaInfo.FullName);
            SetColumnWidth(ref sheet, 2, 20);

            bodyStyle = rowIndex % 2 == 0 ? bodyStyle2 : bodyStyle1;

            cellBody = row.CreateCell(3);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(epaInfo.NickName);
            SetColumnWidth(ref sheet, 3, 12);

            bodyStyle = rowIndex % 2 == 0 ? bodyStyle4 : bodyStyle3;

            cellBody = row.CreateCell(4);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(epaInfo.Category);
            SetColumnWidth(ref sheet, 4, 12);

            cellBody = row.CreateCell(5);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(epaInfo.Group.GroupName);
            SetColumnWidth(ref sheet, 5, 16);

            cellBody = row.CreateCell(6);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(epaInfo.JobTitle.JobTitleName);
            SetColumnWidth(ref sheet, 6, 25);

            bodyStyle = rowIndex % 2 == 0 ? bodyStyle2 : bodyStyle1;

            cellBody = row.CreateCell(7);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(epaInfo.JoinedDate.ToString(Constants.DateFormat));
            SetColumnWidth(ref sheet, 7, 15);
            return sheet;
        }

        private static void CreateBodyStyles(HSSFWorkbook workbook, out ICellStyle bodyStyle1,
            out ICellStyle bodyStyle2, out ICellStyle bodyStyle3, out ICellStyle bodyStyle4)
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

        private static string GetFinalScoreString(EPASummaryPeriodModel period)
        {
            var grade = period != null && period.IsEndPeriod.Equals(true)
                ? (string.IsNullOrEmpty(period.FinalScore)
                    ? Lang.NotAvailable
                    : period.Grade)
                : Lang.Pending;

            return grade;
        }

        private static string GetGradeString(EPASummaryPeriodModel period)
        {
            var finalScore = period != null && period.IsEndPeriod.Equals(true)
                ? (string.IsNullOrEmpty(period.FinalScore)
                    ? Lang.NotAvailable
                    : Math.Round(float.Parse(period.FinalScore), 2).ToString(CultureInfo.InvariantCulture))
                : Lang.Pending;

            return finalScore;
        }

        private static void SetColumnWidth(ref ISheet sheet, int columnIndex, int width)
        {
            sheet.SetColumnWidth(columnIndex, width * ExcelColumnWidth);
        }
    }
}