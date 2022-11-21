namespace SMS.EPA.Services.Implement.ExportFile
{
    using System.Collections.Generic;
    using System.IO;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Core.Utilities.NPOIExtensions;
    using SMS.EPA.Model.JobLevelStandard;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Interfaces.ExportFile;

#pragma warning disable S109 // Magic numbers should not be used
    public class ExportJobLevelAssessmentService : IExportJobLevelAssessmentService
    {
        private const int ExcelColumnWidth = 256;

        public MemoryStream ExportJoblevelAssessmentReport(IList<JobLevelStandardKPI> kpis, IList<JobLevelAssessment> assessments, string title)
        {
            const byte StartBodyRowIndex = 4;
            const byte FontHeight = 18;
            var totalColumns = 16 + kpis.Count * 2;
            var titleHeadersEmployeeInfo = new List<string>
            {
                Lang.Code,
                Lang.FullName,
                Lang.NickName,
                Lang.DOC,
                Lang.LengthOfService,
                Lang.Department,
                Lang.Section,
                Lang.Team,
                Lang.CurrentJobLevel,
                Lang.RequirementMinScore,
            };

            var titleHeadersReview = new List<string>
            {
                Lang.TotalWeight,
                Lang.Difference,
                Lang.SystemPropose
            };

            var workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(Lang.JoblevelAssessmentReportTitle);
            IRow rowTitle = sheet.CreateRow(0);

            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = FontHeight;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICell cell = rowTitle.CreateCell(0);
            cell.SetCellValue(title);
            cell.CellStyle = workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Center;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle.SetFont(font);

            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, totalColumns));

            RenderHeader(kpis, ref workbook, ref sheet, titleHeadersEmployeeInfo, titleHeadersReview);
            RenderContent(assessments, ref workbook, ref sheet);
            sheet.CreateFreezePane(0, StartBodyRowIndex);

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

        private static ICellStyle CreateHeaderStyle(HSSFWorkbook workbook)
        {
            const byte FontHeight = 11;
            IFont font = workbook.CreateFont();
            font.FontHeightInPoints = FontHeight;
            font.Color = HSSFColor.White.Index;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle cellStyle = workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillBackgroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.SetFont(font);
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.RightBorderColor = HSSFColor.White.Index;
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BottomBorderColor = HSSFColor.White.Index;

            return cellStyle;
        }

        private static void StyleMergedRegionHeader(HSSFWorkbook workbook, ISheet sheet, CellRangeAddress region)
        {
            RegionUtil.SetBorderRight((byte)BorderStyle.Thin, region, sheet, workbook);
            RegionUtil.SetRightBorderColor(HSSFColor.White.Index, region, sheet, workbook);

            RegionUtil.SetBorderBottom((byte)BorderStyle.Thin, region, sheet, workbook);
            RegionUtil.SetBottomBorderColor(HSSFColor.White.Index, region, sheet, workbook);
        }

        private static void RenderHeader(
            IList<JobLevelStandardKPI> kpis,
            ref HSSFWorkbook workbook,
            ref ISheet sheet,
            IList<string> titleHeadersEmployeeInfo,
            IList<string> titleHeadersReview)
        {
            var tableHeaderRowSpanIndex = kpis.Count > 0 ? 3 : 2;
            const int tableHeaderColSpanEmployeeInfor = 10;
            var startAssessmentColumnIndex = titleHeadersEmployeeInfo.Count + 1;

            ICellStyle cellStyle = CreateHeaderStyle(workbook);

            IRow row = sheet.CreateRow(1);
            IRow row2 = sheet.CreateRow(2);
            IRow row3 = sheet.CreateRow(3);
            var cell = row.CreateCell(0);
            cell.CellStyle = cellStyle;
            cell.SetCellValue(Lang.SymbolCount);
            sheet.AddMergedRegion(new CellRangeAddress(1, tableHeaderRowSpanIndex, 0, 0));

            var cellEmployeeInfoHeader = row.CreateCell(1);
            cellEmployeeInfoHeader.CellStyle = cellStyle;
            cellEmployeeInfoHeader.SetCellValue(Lang.EmployeeInfo);
            var region = new CellRangeAddress(1, 2, 1, tableHeaderColSpanEmployeeInfor);
            sheet.AddMergedRegion(region);
            StyleMergedRegionHeader(workbook, sheet, region);

            IRow rowEmployeeInfo = sheet.CreateRow(3);
            for (int i = 0; i < titleHeadersEmployeeInfo.Count; i++)
            {
                var cellEmployeeInfo = rowEmployeeInfo.CreateCell(i + 1);
                cellEmployeeInfo.CellStyle = cellStyle;
                cellEmployeeInfo.SetCellValue(titleHeadersEmployeeInfo[i]);
            }

            if (kpis.Count > 0)
            {
                var totolCellJobLevel = kpis.Count * 2 + startAssessmentColumnIndex - 1;
                var cellJobLevelHeader = row.CreateCell(startAssessmentColumnIndex);
                cellJobLevelHeader.CellStyle = cellStyle;
                cellJobLevelHeader.SetCellValue("Job Level Standards (Assessment by Department P.I.C)");
                region = new CellRangeAddress(1, 1, startAssessmentColumnIndex, totolCellJobLevel);
                sheet.AddMergedRegion(region);
                StyleMergedRegionHeader(workbook, sheet, region);

                for (int i = 0; i < kpis.Count; i++)
                {
                    var cellIndex = i == 0 ? startAssessmentColumnIndex : startAssessmentColumnIndex + i * 2;
                    var cellJobLevel = row2.CreateCell(cellIndex);
                    cellJobLevel.CellStyle = cellStyle;
                    cellJobLevel.SetCellValue(kpis[i].Name);
                    region = new CellRangeAddress(2, 2, cellIndex, cellIndex + 1);
                    sheet.AddMergedRegion(region);
                    StyleMergedRegionHeader(workbook, sheet, region);
                }

                var reviewedHeaderIndex = startAssessmentColumnIndex + kpis.Count * 2;
                var cellReviewedHeader = row.CreateCell(reviewedHeaderIndex);
                cellReviewedHeader.CellStyle = cellStyle;
                cellReviewedHeader.SetCellValue(Lang.ReviewbyUnitHead);
                region = new CellRangeAddress(1, 2, reviewedHeaderIndex, reviewedHeaderIndex + 2);
                sheet.AddMergedRegion(region);
                StyleMergedRegionHeader(workbook, sheet, region);

                for (int i = 0; i < titleHeadersReview.Count; i++)
                {
                    var cellReviewedTotalWeight = row3.CreateCell(reviewedHeaderIndex + i);
                    cellReviewedTotalWeight.CellStyle = cellStyle;
                    cellReviewedTotalWeight.SetCellValue(titleHeadersReview[i]);
                }

                var indexCellRemark = reviewedHeaderIndex + titleHeadersReview.Count;
                var indexCellStatus = reviewedHeaderIndex + titleHeadersReview.Count + 1;

                var cellRemark = row.CreateCell(indexCellRemark);
                cellRemark.CellStyle = cellStyle;
                cellRemark.SetCellValue(Lang.Remark);
                sheet.AddMergedRegion(new CellRangeAddress(1, 3, indexCellRemark, indexCellRemark));

                var cellStatus = row.CreateCell(indexCellStatus);
                cellStatus.CellStyle = cellStyle;
                cellStatus.SetCellValue(Lang.Status);
                sheet.AddMergedRegion(new CellRangeAddress(1, 3, indexCellStatus, indexCellStatus));
            }

            for (var i = 0; i <= tableHeaderColSpanEmployeeInfor; i++)
            {
                const int odd = 2;
                var cellEmployeeInfo = row3.CreateCell(startAssessmentColumnIndex + i);
                cellEmployeeInfo.CellStyle = cellStyle;
                cellEmployeeInfo.SetCellValue(i % odd == 0 ? Lang.Level : Lang.Weight);
            }
        }

        private static void RenderContent(
            IList<JobLevelAssessment> assessments,
            ref HSSFWorkbook workbook,
            ref ISheet sheet)
        {
            var rowIndex = 4;
            foreach (var assessment in assessments)
            {
                ICellStyle bodyStyle;
                IRow row;
                ICell cellBody;
                const byte StartAssessmentColumnIndex = 11;
                const byte CellNormalWidth = 12;
                sheet = CreateEmployeeInfoCells(
                    sheet,
                    rowIndex,
                    workbook,
                    assessment,
                    out bodyStyle,
                    out row,
                    out cellBody);

                bodyStyle = CreateBodyStyle(assessment, workbook);
                int cellIndex;
                for (int i = 0; i < assessment.KPIs.Count; i++)
                {
                    cellIndex = StartAssessmentColumnIndex + i * 2;
                    cellBody = row.CreateCell(cellIndex);
                    cellBody.CellStyle = bodyStyle;
                    cellBody.SetCellValue((int?)assessment.KPIs[i].Level);
                    SetColumnWidth(ref sheet, cellIndex, CellNormalWidth);

                    cellBody = row.CreateCell(cellIndex + 1);
                    cellBody.CellStyle = bodyStyle;
                    cellBody.SetCellValue(assessment.KPIs[i].Weight);
                    SetColumnWidth(ref sheet, cellIndex + 1, CellNormalWidth);
                }

                cellIndex = StartAssessmentColumnIndex + assessment.KPIs.Count * 2;
                cellBody = row.CreateCell(cellIndex);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(assessment.TotalWeight);
                SetColumnWidth(ref sheet, cellIndex, CellNormalWidth);
                cellIndex++;

                cellBody = row.CreateCell(cellIndex);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(assessment.SystemPropose);
                SetColumnWidth(ref sheet, cellIndex, CellNormalWidth);
                cellIndex++;

                cellBody = row.CreateCell(cellIndex);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(assessment.NewJobLevel);
                SetColumnWidth(ref sheet, cellIndex, CellNormalWidth);
                cellIndex++;

                cellBody = row.CreateCell(cellIndex);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(assessment.Remark);
                SetColumnWidth(ref sheet, cellIndex, CellNormalWidth);
                cellIndex++;

                cellBody = row.CreateCell(cellIndex);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(Enumerations.GetDescription(assessment.StatusId));
                SetColumnWidth(ref sheet, cellIndex, CellNormalWidth + 3);

                rowIndex++;
            }
        }

        private static ISheet CreateEmployeeInfoCells(
            ISheet sheet,
            int rowIndex,
            HSSFWorkbook workbook,
            JobLevelAssessment assessment,
            out ICellStyle bodyStyle,
            out IRow row,
            out ICell cellBody)
        {
            row = sheet.CreateRow(rowIndex);
            var columnIndex = 0;

            cellBody = row.CreateCell(columnIndex);
            bodyStyle = CreateBodyStyle(assessment, workbook);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(rowIndex - 3);
            SetColumnWidth(ref sheet, columnIndex, 5);
            columnIndex++;

            cellBody = row.CreateCell(columnIndex);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.Code);
            SetColumnWidth(ref sheet, columnIndex, 10);
            columnIndex++;

            cellBody = row.CreateCell(columnIndex);
            bodyStyle = CreateBodyStyle(assessment, workbook);
            bodyStyle.Alignment = HorizontalAlignment.Left;
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.FullName);
            SetColumnWidth(ref sheet, columnIndex, 20);
            columnIndex++;

            cellBody = row.CreateCell(columnIndex);
            bodyStyle = CreateBodyStyle(assessment, workbook);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.Nickname);
            SetColumnWidth(ref sheet, columnIndex, 12);
            columnIndex++;

            cellBody = row.CreateCell(columnIndex);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.JoinedDate.ToString(Constants.DateFormat));
            SetColumnWidth(ref sheet, columnIndex, 16);
            columnIndex++;

            cellBody = row.CreateCell(columnIndex);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.LengthOfService);
            SetColumnWidth(ref sheet, columnIndex, 16);
            columnIndex++;

            cellBody = row.CreateCell(columnIndex);
            bodyStyle = CreateBodyStyle(assessment, workbook);
            bodyStyle.Alignment = HorizontalAlignment.Left;
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.Department);
            SetColumnWidth(ref sheet, columnIndex, 16);
            columnIndex++;

            cellBody = row.CreateCell(columnIndex);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.Section);
            SetColumnWidth(ref sheet, columnIndex, 25);
            columnIndex++;

            cellBody = row.CreateCell(columnIndex);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.Team);
            SetColumnWidth(ref sheet, columnIndex, 18);
            columnIndex++;

            cellBody = row.CreateCell(columnIndex);
            bodyStyle = CreateBodyStyle(assessment, workbook);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.CurrentJobLevel);
            SetColumnWidth(ref sheet, columnIndex, 12);
            columnIndex++;

            cellBody = row.CreateCell(columnIndex);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.RequirementMinScore);
            SetColumnWidth(ref sheet, columnIndex, 12);

            return sheet;
        }

        private static ICellStyle CreateBodyStyle(JobLevelAssessment assessment, HSSFWorkbook workbook)
        {
            var bodyStyle = workbook.CreateCellStyle();
            bodyStyle.Alignment = HorizontalAlignment.Center;
            bodyStyle.VerticalAlignment = VerticalAlignment.Center;
            if (assessment.NewJobLevelId.HasValue)
            {
                if (assessment.SystemPropose < 0)
                {
                    bodyStyle.FillForegroundColor = HSSFColor.LightOrange.Index;
                    bodyStyle.FillPattern = FillPattern.SolidForeground;
                    bodyStyle.FillBackgroundColor = HSSFColor.LightOrange.Index;
                }
                else
                {
                    if (assessment.SystemPropose > 0 && assessment.CurrentJobLevelId != assessment.NewJobLevelId)
                    {
                        bodyStyle.FillForegroundColor = HSSFColor.LightGreen.Index;
                        bodyStyle.FillPattern = FillPattern.SolidForeground;
                        bodyStyle.FillBackgroundColor = HSSFColor.LightGreen.Index;
                    }
                }
            }

            return bodyStyle;
        }

        private static void SetColumnWidth(ref ISheet sheet, int columnIndex, int width)
        {
            sheet.SetColumnWidth(columnIndex, width * ExcelColumnWidth);
        }
    }
}