namespace SMS.EPA.Services.Implement.ExportFile
{
    using Core.Constants;
    using Core.Enumerations;
    using Interfaces;
    using Model;
    using NPOI.HSSF.UserModel;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    [SuppressMessage("Major Code Smell", "S109:Magic numbers should not be used", Justification = "<Pending>")]
    public abstract class EPADetailReportServiceBase : IEPADetailReportService
    {
        protected const int ExcelColumnWidth = 256;

        private readonly ICommonService commonService;
        private readonly IEmployeeEPAService employeeEPAService;

        protected EPADetailReportServiceBase() : this(null, null)
        {
        }

        protected EPADetailReportServiceBase(ICommonService commonService, IEmployeeEPAService employeeEPAService)
        {
            this.commonService = commonService ?? new CommonService();
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
        }

        public abstract IList<string> GetColumnsName { get; }

        protected ICellStyle BodyStyle { get; set; }

        protected IList<string> ColumnsName { get; set; }

        protected int CurrentRow { get; set; }

        protected EPADetailModel EpaDetail { get; set; }

        protected ICellStyle EvenRedStyle { get; set; }

        protected ICellStyle EvenStyleCenter { get; set; }

        protected ICellStyle EvenStyleLeft { get; set; }

        protected ICellStyle OddRedStyle { get; set; }

        protected ICellStyle OddStyleCenter { get; set; }

        protected ICellStyle OddStyleLeft { get; set; }

        protected IFont RedFont { get; set; }

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
            string evaluatorScore = Lang.NotAvailable;

            if (epaInfo.Process.Evaluators.First().StatusId == (int)ProcessStatusType.InProgress)
            {
                evaluatorScore = epaInfo.Process.AverageScoreByEvaluators?.ToString() ?? Lang.NotAvailable;
            }

            if (epaInfo.Process.Evaluators.First().StatusId == (int)ProcessStatusType.Closed)
            {
                evaluatorScore = epaInfo.Process.AverageScoreByEvaluators?.ToString() ?? Lang.NotAvailable;
            }

            return evaluatorScore;
        }

        protected static string GetReviewerScore(EPAModel epaInfo)
        {
            string reviewerScore = Lang.NotAvailable;

            if (epaInfo.Process.Reviewers.First().StatusId == (int)ProcessStatusType.InProgress)
            {
                reviewerScore = epaInfo.Process.AverageScoreByReviewers?.ToString() ?? Lang.NotAvailable;
            }

            if (epaInfo.Process.Reviewers.First().StatusId == (int)ProcessStatusType.Closed)
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
            string selfScore = Lang.NotAvailable;

            if (epaInfo.Process.Self.StatusId == (int)ProcessStatusType.InProgress)
            {
                float? score = !string.IsNullOrEmpty(epaInfo.Process.Self.TotalScore)
                    ? float.Parse(epaInfo.Process.Self.TotalScore)
                    : 0;
                selfScore = Math.Round(score.Value, 2).ToString(CultureInfo.InvariantCulture);
            }

            if (epaInfo.Process.Self.StatusId == (int)ProcessStatusType.Closed)
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

        protected string GetGrade(EPAModel epaInfo, string finalScore)
        {
            var standardGrades = commonService.GetStandardGrade().ToList();

            float score;

            float? performanceScore = null;
            if (float.TryParse(finalScore, out score) && Math.Abs(score) > Constants.Tolerance)
            {
                performanceScore = score - epaInfo.DisciplineScore;
            }

            return epaInfo.FinalScoreToExport != string.Empty
                ? epaInfo.Grade
                : employeeEPAService.ConvertScoreToGrade(
                    performanceScore,
                    epaInfo.DisciplineScore,
                    epaInfo.HasDisciplineScore,
                    standardGrades);
        }

        protected void SetColumnWidth(int columnIndex, int width)
        {
            Sheet.SetColumnWidth(columnIndex, width * ExcelColumnWidth);
        }

        protected int WriteCommonCells(EPAModel epaInfo, IRow row, int currentRow)
        {
            var currentColumn = 0;

            var cell = row.CreateCell(currentColumn);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(currentRow);
            SetColumnWidth(currentColumn++, 5);

            cell = row.CreateCell(currentColumn);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(epaInfo.Employee.Code);
            SetColumnWidth(currentColumn++, 10);

            BodyStyle = ApplyEvenOddBodyStyle(EvenStyleCenter, OddStyleCenter);

            cell = row.CreateCell(currentColumn);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(epaInfo.Employee.NickName);
            SetColumnWidth(currentColumn++, 12);

            BodyStyle = ApplyEvenOddBodyStyle(EvenStyleLeft, OddStyleLeft);

            cell = row.CreateCell(currentColumn);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(epaInfo.Employee.FullName);
            SetColumnWidth(currentColumn++, 20);

            BodyStyle = ApplyEvenOddBodyStyle(EvenStyleLeft, OddStyleLeft);

            cell = row.CreateCell(currentColumn);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(epaInfo.Employee.Category);
            SetColumnWidth(currentColumn++, 12);

            cell = row.CreateCell(currentColumn);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(epaInfo.Employee.DepartmentName);
            SetColumnWidth(currentColumn++, 16);

            cell = row.CreateCell(currentColumn);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(epaInfo.Employee.Section);
            SetColumnWidth(currentColumn++, 16);

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
            if (epaInfo.Process.Self.StatusId == (int)ProcessStatusType.InProgress)
            {
                BodyStyle = ApplyEvenOddBodyStyle(EvenRedStyle, OddRedStyle);
            }

            var cell = row.CreateCell(localCurrentColumn++);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(GetScoreByPhaseSelected(
                EpaDetail.PhaseSelecteds,
                score.SelfScore,
                (byte)ProcessType.Self));

            BodyStyle = epaInfo.Process.Evaluators.First().StatusId == (int)ProcessStatusType.InProgress
                ? ApplyEvenOddBodyStyle(EvenRedStyle, OddRedStyle)
                : ApplyEvenOddBodyStyle(EvenStyleCenter, OddStyleCenter);

            cell = row.CreateCell(localCurrentColumn++);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(GetScoreByPhaseSelected(
                EpaDetail.PhaseSelecteds,
                score.EvaluatorScore,
                (byte)ProcessType.Evaluator));

            BodyStyle = epaInfo.Process.Reviewers.First().StatusId == (int)ProcessStatusType.InProgress
                ? ApplyEvenOddBodyStyle(EvenRedStyle, OddRedStyle)
                : ApplyEvenOddBodyStyle(EvenStyleCenter, OddStyleCenter);

            cell = row.CreateCell(localCurrentColumn++);
            cell.CellStyle = BodyStyle;
            cell.SetCellValue(GetScoreByPhaseSelected(
                EpaDetail.PhaseSelecteds,
                score.ReviewerScore,
                (byte)ProcessType.Reviewer));

            return localCurrentColumn;
        }

        protected void WriteTitleSheet()
        {
            IRow rowTitle = Sheet.CreateRow(CurrentRow);
            IFont font = Workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICell cell = rowTitle.CreateCell(0);
            cell.SetCellValue(SheetTittle + " - " + EpaDetail.Period.Name);
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