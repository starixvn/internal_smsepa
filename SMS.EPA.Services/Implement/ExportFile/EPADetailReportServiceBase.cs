namespace SMS.EPA.Services.Implement.ExportFile
{
    using System;
    using Core.Enumerations;
    using Model;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using Resources;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using Core.Constants;
    using Interfaces;

    public abstract class EPADetailReportServiceBase : IEPADetailReportService
    {
        protected const int ExcelColumnWidth = 256;

        protected IFont RedFont { get; set; }

        protected ICellStyle EvenStyleCenter { get; set; }

        protected ICellStyle OddStyleCenter { get; set; }

        protected ICellStyle EvenRedStyle { get; set; }

        protected ICellStyle OddRedStyle { get; set; }

        protected ICellStyle EvenStyleLeft { get; set; }

        protected ICellStyle OddStyleLeft { get; set; }

        protected ICellStyle BodyStyle { get; set; }

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

        protected static string GetEvaluatorScore(EPAModel epaInfo)
        {
            string evaluatorScore = string.Empty;

            if (epaInfo.Process.Evaluators.First().StatusEndProcess == (int)ProcessStatusType.InProgress)
            {
                evaluatorScore = epaInfo.Process.AverageScoreByEvaluators?.ToString() ?? "0";
            }

            if (epaInfo.Process.Evaluators.First().StatusEndProcess == (int)ProcessStatusType.Closed)
            {
                evaluatorScore = epaInfo.Process.AverageScoreByEvaluators?.ToString() ?? Lang.NotAvailable;
            }

            return evaluatorScore;
        }

        protected static string GetFinalScore(EPAModel epaInfo)
        {
            var finalScore = epaInfo.Period.IsEndPeriod
                ? (string.IsNullOrEmpty(epaInfo.FinalScore)
                    ? Lang.NotAvailable
                    : Math.Round(float.Parse(epaInfo.FinalScore), 2).ToString(CultureInfo.InvariantCulture))
                : Lang.Pending;

            return finalScore;
        }

        protected static string GetGrade(EPAModel epaInfo)
        {
            var grade = epaInfo.Period.IsEndPeriod
                ? (string.IsNullOrEmpty(epaInfo.FinalScore) ? Lang.NotAvailable : epaInfo.Grade)
                : Lang.Pending;

            return grade;
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

        protected static string GetScoreByPhaseSelected(
            IEnumerable<EvaluationPhaseModel> phaseSelecteds,
            string score,
            int phaseId)
        {
            return phaseSelecteds.Count(x => x.PhaseId == phaseId).Equals(1) ? score : "-";
        }

        protected static string GetSelfScore(EPAModel epaInfo)
        {
            string selfScore = string.Empty;

            if (epaInfo.Process.Self.StatusEndProcess == (int)ProcessStatusType.InProgress)
            {
                float? score = !string.IsNullOrEmpty(epaInfo.Process.Self.TotalScore)
                    ? float.Parse(epaInfo.Process.Self.TotalScore)
                    : 0;
                selfScore = Math.Round(score.Value, 2).ToString(CultureInfo.InvariantCulture);
            }

            if (epaInfo.Process.Self.StatusEndProcess == (int)ProcessStatusType.Closed)
            {
                selfScore = !string.IsNullOrEmpty(epaInfo.Process.Self.TotalScore)
                    ? Math.Round(float.Parse(epaInfo.Process.Self.TotalScore), 2)
                        .ToString(CultureInfo.InvariantCulture)
                    : Lang.NotAvailable;
            }

            return selfScore;
        }

        protected abstract dynamic CalculateScore(EPAModel epaInfo);

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

        [SuppressMessage(
            "StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        protected int WriteCommonCells(EPAModel epaInfo, IRow row)
        {
            var currentColumn = 0;

            var cell = row.CreateCell(currentColumn);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(1);
            SetColumnWidth(currentColumn++, 5);

            cell = row.CreateCell(currentColumn);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(epaInfo.Employee.Code);
            SetColumnWidth(currentColumn++, 10);

            BodyStyle = ApplyEvenOddBodyStyle(EvenStyleLeft, OddStyleLeft);

            cell = row.CreateCell(currentColumn);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(epaInfo.Employee.FullName);
            SetColumnWidth(currentColumn++, 20);

            BodyStyle = ApplyEvenOddBodyStyle(EvenStyleCenter, OddStyleCenter);

            cell = row.CreateCell(currentColumn);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(epaInfo.Employee.NickName);
            SetColumnWidth(currentColumn++, 12);

            BodyStyle = ApplyEvenOddBodyStyle(EvenStyleLeft, OddStyleLeft);

            cell = row.CreateCell(currentColumn);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(epaInfo.Employee.Category);
            SetColumnWidth(currentColumn++, 12);

            cell = row.CreateCell(currentColumn);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(epaInfo.Employee.GroupName);
            SetColumnWidth(currentColumn++, 16);

            cell = row.CreateCell(currentColumn);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(epaInfo.Employee.JobTitleName);
            SetColumnWidth(currentColumn++, 25);

            BodyStyle = ApplyEvenOddBodyStyle(EvenStyleCenter, OddStyleCenter);

            cell = row.CreateCell(currentColumn);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(epaInfo.Employee.JoinedDate.ToString(Constants.DateFormat));
            SetColumnWidth(currentColumn++, 15);

            return currentColumn;
        }

        protected int WriteScoreCells(EPAModel epaInfo, int currentColumn, IRow row, dynamic score)
        {
            int localCurrentColumn = currentColumn;
            if (!epaInfo.Process.Self.IsEndProcess)
            {
                BodyStyle = ApplyEvenOddBodyStyle(EvenRedStyle, OddRedStyle);
            }

            var cell = row.CreateCell(localCurrentColumn++);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(GetScoreByPhaseSelected(
                EpaDetail.PhaseSelecteds,
                score.SelfScore,
                (int)ProcessType.Self));

            if (!epaInfo.Process.Evaluators.First().IsEndProcess)
            {
                BodyStyle = ApplyEvenOddBodyStyle(EvenRedStyle, OddRedStyle);
            }

            cell = row.CreateCell(localCurrentColumn++);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(GetScoreByPhaseSelected(
                EpaDetail.PhaseSelecteds,
                score.EvaluatorScore,
                (int)ProcessType.Evaluator));

            if (!epaInfo.Process.Reviewers.First().IsEndProcess)
            {
                BodyStyle = ApplyEvenOddBodyStyle(EvenRedStyle, OddRedStyle);
            }

            cell = row.CreateCell(localCurrentColumn++);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(GetScoreByPhaseSelected(
                EpaDetail.PhaseSelecteds,
                score.ReviewerScore,
                (int)ProcessType.Reviewer));

            return localCurrentColumn;
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

        protected void CreateDefaultStyles()
        {
            RedFont = Workbook.CreateFont();
            RedFont.Color = HSSFColor.Red.Index;

            EvenStyleCenter = CreateEvenStyleCenter();
            OddStyleCenter = CreateOddStyleCenter();

            EvenRedStyle = CreateEvenStyleCenter();
            EvenRedStyle.SetFont(RedFont);

            OddRedStyle = CreateOddStyleCenter();
            OddRedStyle.SetFont(RedFont);

            EvenStyleLeft = CreateEvenStyleLeft();
            OddStyleLeft = CreateOddStyleLeft();
        }
    }
}
