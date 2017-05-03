namespace SMS.EPA.Services.Implement.ExportFile
{
    using Core.Enumerations;
    using Model;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using Resources;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Interfaces;

    public abstract class EPADetailReportServiceBase : IEPADetailReportService
    {
        protected const int ExcelColumnWidth = 256;

        public abstract IList<string> GetColumnsName { get; }

        protected IList<string> ColumnsName { get; set; }

        protected int CurrentRow { get; set; }

        protected EPADetailModel EpaDetail { get; set; }

        protected ISheet Sheet { get; set; }

        protected string SheetTittle { get; set; }

        protected HSSFWorkbook Workbook { get; set; }

        public abstract void Render(EPADetailModel epaDetail);

        public abstract void WriteContentTable();

        public void Export(EPADetailModel epaDetail)
        {
            EpaDetail = epaDetail;
            Workbook = new HSSFWorkbook();
            Sheet = Workbook.CreateSheet(SheetTittle);
            ColumnsName = GetColumnsName;
            WriteTitleSheet();
            CurrentRow += 2;

            WriteTitleTable();
            CurrentRow++;

            WriteContentTable();
            Sheet.CreateFreezePane(0, 3);

            using (var output = new MemoryStream())
            {
                Workbook.Write(output);
            }
        }

        protected static string GetReviewerScore(EPAModel epaInfo)
        {
            string reviewerScore = string.Empty;

            if (epaInfo.Process.Reviewers.First().StatusEndProcess == (int)ProcessStatusType.InProgress)
            {
                reviewerScore = epaInfo.Process.AverageScoreByReviewers?.ToString() ?? "0";
            }

            if (epaInfo.Process.Reviewers.First().StatusEndProcess == (int)ProcessStatusType.Closed)
            {
                reviewerScore = epaInfo.Process.AverageScoreByReviewers?.ToString() ?? Lang.NotAvailable;
            }

            return reviewerScore;
        }

        protected ICellStyle ApplyEvenOddBodyStyle(ICellStyle evenStyle, ICellStyle oddStyle)
        {
            return CurrentRow % 2 == 0 ? evenStyle : oddStyle;
        }

        protected ICellStyle CreateEvenStyleCenter()
        {
            ICellStyle evenStyle = Workbook.CreateCellStyle();
            evenStyle.Alignment = HorizontalAlignment.Center;
            evenStyle.VerticalAlignment = VerticalAlignment.Center;
            evenStyle.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            evenStyle.FillPattern = FillPattern.SolidForeground;
            evenStyle.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;

            return evenStyle;
        }

        protected ICellStyle CreateEvenStyleLeft()
        {
            ICellStyle evenStyle = Workbook.CreateCellStyle();
            evenStyle.Alignment = HorizontalAlignment.Left;
            evenStyle.VerticalAlignment = VerticalAlignment.Center;
            evenStyle.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            evenStyle.FillPattern = FillPattern.SolidForeground;
            evenStyle.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;

            return evenStyle;
        }

        protected ICellStyle CreateOddStyleCenter()
        {
            ICellStyle oddStyle = Workbook.CreateCellStyle();
            oddStyle.Alignment = HorizontalAlignment.Center;
            oddStyle.VerticalAlignment = VerticalAlignment.Center;

            return oddStyle;
        }

        protected ICellStyle CreateOddStyleLeft()
        {
            ICellStyle oddStyle = Workbook.CreateCellStyle();
            oddStyle.Alignment = HorizontalAlignment.Left;
            oddStyle.VerticalAlignment = VerticalAlignment.Center;

            return oddStyle;
        }

        protected void SetColumnWidth(int columnIndex, int width)
        {
            Sheet.SetColumnWidth(columnIndex, width * ExcelColumnWidth);
        }

        protected void WriteTitleSheet()
        {
            IRow rowTitle = Sheet.CreateRow(CurrentRow);

            IFont font = Workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICell cell = rowTitle.CreateCell(0);
            cell.SetCellValue(SheetTittle);
            cell.CellStyle = Workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Center;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle.SetFont(font);

            Sheet.AddMergedRegion(new CellRangeAddress(CurrentRow, CurrentRow, 0, ColumnsName.Count - 1));
        }

        protected void WriteTitleTable()
        {
            IFont font = Workbook.CreateFont();
            font.FontHeightInPoints = 11;
            font.Color = HSSFColor.White.Index;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle cellStyle = Workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillBackgroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.SetFont(font);

            IRow row = Sheet.CreateRow(CurrentRow);
            for (int i = 0; i < ColumnsName.Count; i++)
            {
                var cell = row.CreateCell(i);
                cell.CellStyle = cellStyle;
                cell.SetCellValue(ColumnsName[i]);
                Sheet.AutoSizeColumn(i);
            }
        }
    }
}
