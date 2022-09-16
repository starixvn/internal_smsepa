namespace SMS.EPA.Services.Implement.ExportFile
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.Formula.Functions;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Utilities.NPOIExtensions;
    using SMS.EPA.Model;
    using SMS.EPA.Model.JobLevelStandard;
    using SMS.EPA.Resources;
    using SMS.EPA.Services.Interfaces.ExportFile;
    public class ExportJobLevelAssessmentService : IExportJobLevelAssessmentService
    {
        private const int ExcelColumnWidth = 256;

        public MemoryStream ExportJoblevelAssessmentReport(IList<JobLevelStandardKPI> kpis, IList<JobLevelAssessment> assessments, string title)
        {
            const short fontHeight = 18;
            var countLastColumn = 10 + (kpis.Count > 0 ? kpis.Count * 2 + 4 : 0);
            IList<string> titleHeadersEmployeeInfo = new List<string>
            {
                Lang.Code,
                Lang.FullName,
                Lang.NickName,
                Lang.DOC,
                Lang.Department,
                Lang.Section,
                Lang.Team,
                Lang.CurrentJobLevel,
                Lang.RequirementMinScore,
            };

            IList<string> titleHeadersReview = new List<string>
            {
                Lang.TotalWeight,
                Lang.SystemPropose,
                Lang.NewJobLevel
            };

            HSSFWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet(title);
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

            sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, countLastColumn));

            RenderHeaderJoblevelAsssessmentReport(kpis, ref workbook, ref sheet, titleHeadersEmployeeInfo, titleHeadersReview);
            RenderContentRowJoblevelAsssessmentReport(assessments, ref workbook, ref sheet);

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

        private static void RenderHeaderJoblevelAsssessmentReport(
            IList<JobLevelStandardKPI> kpis,
            ref HSSFWorkbook workbook,
            ref ISheet sheet,
            IList<string> titleHeadersEmployeeInfo,
            IList<string> titleHeadersReview)
        {
            var tableHeaderRowSpanIndex = kpis.Count > 0 ? 3 : 2;
            var tableHeaderRowSpanEmployeeInfor = kpis.Count > 0 ? 2 : 1;
            const int tableHeaderColSpanEmployeeInfor = 9;
            var startTitleHeaderJobLevel = titleHeadersEmployeeInfo.Count + 1;
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
            sheet.AddMergedRegion(new CellRangeAddress(1, tableHeaderRowSpanEmployeeInfor, 1, tableHeaderColSpanEmployeeInfor));

            IRow rowEmployeeInfo = sheet.CreateRow(tableHeaderRowSpanEmployeeInfor + 1);
            for (int i = 0; i < titleHeadersEmployeeInfo.Count; i++)
            {
                var cellEmployeeInfo = rowEmployeeInfo.CreateCell(i + 1);
                cellEmployeeInfo.CellStyle = cellStyle;
                cellEmployeeInfo.SetCellValue(titleHeadersEmployeeInfo[i]);
            }

            if(kpis.Count > 0)
            {
                var totolCellJobLevel = kpis.Count * 2 + startTitleHeaderJobLevel -1;
                var cellJobLevelHeader = row.CreateCell(startTitleHeaderJobLevel);
                cellJobLevelHeader.CellStyle = cellStyle;
                cellJobLevelHeader.SetCellValue(Lang.JobLevelStandards);
                sheet.AddMergedRegion(new CellRangeAddress(1, 1, startTitleHeaderJobLevel, totolCellJobLevel));

                for (int i = 0; i < kpis.Count; i++)
                {
                    var cellIndex = i == 0 ? startTitleHeaderJobLevel : startTitleHeaderJobLevel + i * 2;
                    var cellJobLevel = row2.CreateCell(cellIndex);
                    cellJobLevel.CellStyle = cellStyle;
                    cellJobLevel.SetCellValue(kpis[i].Name);
                    sheet.AddMergedRegion(new CellRangeAddress(2, 2, cellIndex, cellIndex + 1));
                }

                var reviewedHeaderIndex = startTitleHeaderJobLevel + kpis.Count * 2;
                var cellReviewedHeader = row.CreateCell(reviewedHeaderIndex);
                cellReviewedHeader.CellStyle = cellStyle;
                cellReviewedHeader.SetCellValue(Lang.ReviewbyUnitHead);
                sheet.AddMergedRegion(new CellRangeAddress(1, 2, reviewedHeaderIndex, reviewedHeaderIndex + 2));

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

            for (int i = 0; i <= tableHeaderColSpanEmployeeInfor; i++)
            {
                const int odd = 2;
                var cellEmployeeInfo = row3.CreateCell(startTitleHeaderJobLevel + i);
                cellEmployeeInfo.CellStyle = cellStyle;
                cellEmployeeInfo.SetCellValue(i % odd == 0 ? Lang.Level : Lang.Weight);
            }
        }

        private static void RenderContentRowJoblevelAsssessmentReport(
            IList<JobLevelAssessment> assessments,
            ref HSSFWorkbook workbook,
            ref ISheet sheet)
        {
            var rowIndex = 4;
            IFont redFont = workbook.CreateFont();
            redFont.Color = HSSFColor.Red.Index;
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

            foreach (var assessment in assessments)
            {
                ICellStyle bodyStyle;
                IRow row;
                ICell cellBody;
                const int cellNormalWidth = 12;
                sheet = CreateEmployeeInfoCells(
                    sheet,
                    rowIndex,
                    bodyStyle1,
                    bodyStyle2,
                    bodyStyle3,
                    bodyStyle4,
                    assessment,
                    out bodyStyle,
                    out row,
                    out cellBody);

                bodyStyle = rowIndex % 2 == 0 ? bodyStyle2 : bodyStyle1;

                for (int i = 0; i < assessment.KPIs.Count; i++)
                {
                    var cellIndex = i == 0 ? 10 : 10 + i * 2;
                    cellBody = row.CreateCell(cellIndex);
                    cellBody.CellStyle = bodyStyle;
                    cellBody.SetCellValue(assessment.KPIs[i].Level);
                    SetColumnWidth(ref sheet, cellIndex, cellNormalWidth);

                    cellBody = row.CreateCell(cellIndex + 1);
                    cellBody.CellStyle = bodyStyle;
                    cellBody.SetCellValue(assessment.KPIs[i].Weight);
                    SetColumnWidth(ref sheet, cellIndex + 1, cellNormalWidth);
                }

                var startCellReview = 10 + assessment.KPIs.Count * 2;
                cellBody = row.CreateCell(startCellReview);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(assessment.TotalWeight);
                SetColumnWidth(ref sheet, startCellReview, cellNormalWidth);

                cellBody = row.CreateCell(startCellReview + 1);
                cellBody.CellStyle = bodyStyle;
                //if (assessment.SystemPropose < 0)
                //{
                //    cellBody.CellStyle.SetFont(redFont);
                //}
                cellBody.SetCellValue(assessment.SystemPropose);
                SetColumnWidth(ref sheet, startCellReview + 1, cellNormalWidth);

                cellBody = row.CreateCell(startCellReview + 2);
                cellBody.CellStyle = bodyStyle;
                //if (assessment.SystemPropose > 0 && assessment.CurrentJobLevelId != assessment.NewJobLevelId)
                //{
                //    cellBody.CellStyle.SetFont(redFont);
                //}
                cellBody.SetCellValue(assessment.NewJobLevel);
                SetColumnWidth(ref sheet, startCellReview + 2, cellNormalWidth);

                bodyStyle = rowIndex % 2 == 0 ? bodyStyle4 : bodyStyle3;
                cellBody = row.CreateCell(startCellReview + 3);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(assessment.Remark);
                SetColumnWidth(ref sheet, startCellReview + 3, cellNormalWidth);

                cellBody = row.CreateCell(startCellReview + 4);
                cellBody.CellStyle = bodyStyle;
                cellBody.SetCellValue(assessment.StatusId.ToString());
                SetColumnWidth(ref sheet, startCellReview + 4, cellNormalWidth + 3);


                rowIndex++;
            }
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
        private static ISheet CreateEmployeeInfoCells(
            ISheet sheet,
            int rowIndex,
            ICellStyle bodyStyle1,
            ICellStyle bodyStyle2,
            ICellStyle bodyStyle3,
            ICellStyle bodyStyle4,
            JobLevelAssessment assessment,
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
            cellBody.SetCellValue(assessment.Code);
            SetColumnWidth(ref sheet, 1, 10);

            bodyStyle = rowIndex % 2 == 0 ? bodyStyle4 : bodyStyle3;
            cellBody = row.CreateCell(2);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.FullName);
            SetColumnWidth(ref sheet, 2, 20);

            cellBody = row.CreateCell(3);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.Nickname);
            SetColumnWidth(ref sheet, 3, 12);

            bodyStyle = rowIndex % 2 == 0 ? bodyStyle2 : bodyStyle1;
            cellBody = row.CreateCell(4);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.JoinedDate.ToString(Constants.DateFormat));
            SetColumnWidth(ref sheet, 4, 16);

            bodyStyle = rowIndex % 2 == 0 ? bodyStyle4 : bodyStyle3;
            cellBody = row.CreateCell(5);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.Department);
            SetColumnWidth(ref sheet, 5, 16);

            cellBody = row.CreateCell(6);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.Section);
            SetColumnWidth(ref sheet, 6, 25);

            cellBody = row.CreateCell(7);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.Team);
            SetColumnWidth(ref sheet, 7, 18);

            bodyStyle = rowIndex % 2 == 0 ? bodyStyle2 : bodyStyle1;
            cellBody = row.CreateCell(8);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.CurrentJobLevel);
            SetColumnWidth(ref sheet, 8, 12);

            cellBody = row.CreateCell(9);
            cellBody.CellStyle = bodyStyle;
            cellBody.SetCellValue(assessment.RequirementMinScore);
            SetColumnWidth(ref sheet, 9, 12);

            return sheet;
        }

        private static void SetColumnWidth(ref ISheet sheet, int columnIndex, int width)
        {
            sheet.SetColumnWidth(columnIndex, width * ExcelColumnWidth);
        }
    }
}
