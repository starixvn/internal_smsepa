namespace SMS.EPA.Services.Implement
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
    using System.Web;

    public class EPADetailReportService : IEPADetailReportService
    {
        private const int ExcelColumnWidth = 256;

        private HSSFWorkbook _workbook;
        private ISheet _sheet;
        private int _currentRow;
        private IList<string> _columnsName;

        public EPADetailModel EpaDetail { get; set; }

        public void Export(EPADetailModel epaDetail)
        {
            EpaDetail = epaDetail;
            _workbook = new HSSFWorkbook();
            _sheet = _workbook.CreateSheet(Lang.EPADetailReportSheet);
            _columnsName = CreateColumnsName();
            WriteTitleSheet();
            _currentRow += 2;

            WriteTitleTable();
            _currentRow++;

            WriteContentTable();
            _sheet.CreateFreezePane(0, 3);

            using (var output = new MemoryStream())
            {
                _workbook.Write(output);
            }
        }

        public void Render(EPADetailModel epaDetail)
        {
            if (_workbook == null)
            {
                Export(epaDetail);
            }

            HttpResponse response = HttpContext.Current.Response;

            response.Clear();
            response.ContentType = "application/vnd.ms-excel";

            response.AddHeader(
                "Content-Disposition",
                string.IsNullOrEmpty(epaDetail.EpaYear) || string.IsNullOrEmpty(epaDetail.EpaEvaluationPeriod)
                    ? $"attachment; filename={Lang.EPADetailReportName}.xls"
                    : $"attachment; filename={epaDetail.EpaYear}_{epaDetail.EpaEvaluationPeriod}_{Lang.EPADetailReportName}.xls");

            _workbook.Write(response.OutputStream);
        }

        private void WriteTitleSheet()
        {
            IRow rowTitle = _sheet.CreateRow(_currentRow);

            IFont font = _workbook.CreateFont();
            font.FontHeightInPoints = 18;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICell cell = rowTitle.CreateCell(0);
            cell.SetCellValue(Lang.EPADetailReportSheet);
            cell.CellStyle = _workbook.CreateCellStyle();
            cell.CellStyle.Alignment = HorizontalAlignment.Center;
            cell.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            cell.CellStyle.SetFont(font);

            _sheet.AddMergedRegion(new CellRangeAddress(_currentRow, _currentRow, 0, _columnsName.Count - 1));
        }

        private void WriteTitleTable()
        {
            IFont font = _workbook.CreateFont();
            font.FontHeightInPoints = 11;
            font.Color = HSSFColor.White.Index;
            font.Boldweight = (short)FontBoldWeight.Bold;

            ICellStyle cellStyle = _workbook.CreateCellStyle();
            cellStyle.FillForegroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.FillPattern = FillPattern.SolidForeground;
            cellStyle.FillBackgroundColor = HSSFColor.Grey80Percent.Index;
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.SetFont(font);

            IRow row = _sheet.CreateRow(_currentRow);
            for (int i = 0; i < _columnsName.Count; i++)
            {
                var cell = row.CreateCell(i);
                cell.CellStyle = cellStyle;
                cell.SetCellValue(_columnsName[i]);
                _sheet.AutoSizeColumn(i);
            }
        }

        [SuppressMessage(
            "StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage(
            "Microsoft.Maintainability",
            "CA1502:AvoidExcessiveComplexity",
            Justification = "Reviewed. Suppression is OK here.")]
        private void WriteContentTable()
        {
            IFont redFont = _workbook.CreateFont();
            redFont.Color = HSSFColor.Red.Index;

            ICellStyle evenStyleCenter = CreateEvenStyleCenter();
            ICellStyle oddStyleCenter = CreateOddStyleCenter();

            ICellStyle evenRedStyle = CreateEvenStyleCenter();
            evenRedStyle.SetFont(redFont);

            ICellStyle oddRedStyle = CreateOddStyleCenter();
            oddRedStyle.SetFont(redFont);

            ICellStyle evenStyleLeft = CreateEvenStyleLeft();
            ICellStyle oddStyleLeft = CreateOddStyleLeft();

            var index = 1;
            foreach (var epaInfo in EpaDetail.EpaInfos)
            {
                var currentColumn = 0;

                ICellStyle bodyStyle = ApplyEvenOddBodyStyle(evenStyleCenter, oddStyleCenter);

                IRow row = _sheet.CreateRow(_currentRow);

                var cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(index++);
                SetColumnWidth(currentColumn++, 5);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(epaInfo.Employee.Code);
                SetColumnWidth(currentColumn++, 10);

                bodyStyle = ApplyEvenOddBodyStyle(evenStyleLeft, oddStyleLeft);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(epaInfo.Employee.FullName);
                SetColumnWidth(currentColumn++, 20);

                bodyStyle = ApplyEvenOddBodyStyle(evenStyleCenter, oddStyleCenter);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(epaInfo.Employee.NickName);
                SetColumnWidth(currentColumn++, 12);

                bodyStyle = ApplyEvenOddBodyStyle(evenStyleLeft, oddStyleLeft);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(epaInfo.Employee.Category);
                SetColumnWidth(currentColumn++, 12);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(epaInfo.Employee.GroupName);
                SetColumnWidth(currentColumn++, 16);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(epaInfo.Employee.JobTitleName);
                SetColumnWidth(currentColumn++, 25);

                bodyStyle = ApplyEvenOddBodyStyle(evenStyleCenter, oddStyleCenter);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(epaInfo.Employee.JoinedDate.ToString(Constants.DateFormat));
                SetColumnWidth(currentColumn++, 15);

                bodyStyle = ApplyEvenOddBodyStyle(evenStyleLeft, oddStyleLeft);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(epaInfo.Period.TargetTypeName);

                bodyStyle = ApplyEvenOddBodyStyle(evenStyleCenter, oddStyleCenter);

                if (!epaInfo.Process.Self.IsEndProcess)
                {
                    bodyStyle = ApplyEvenOddBodyStyle(evenRedStyle, oddRedStyle);
                }

                dynamic score = CalculateScore(epaInfo);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(EpaDetail.PhaseSelecteds, score.SelfScore,
                    (int)ProcessType.Self));

                if (!epaInfo.Process.Evaluators.First().IsEndProcess)
                {
                    bodyStyle = ApplyEvenOddBodyStyle(evenRedStyle, oddRedStyle);
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(EpaDetail.PhaseSelecteds, score.EvaluatorScore,
                    (int)ProcessType.Evaluator));

                if (!epaInfo.Process.Reviewers.First().IsEndProcess)
                {
                    bodyStyle = ApplyEvenOddBodyStyle(evenRedStyle, oddRedStyle);
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(EpaDetail.PhaseSelecteds, score.ReviewerScore,
                    (int)ProcessType.Reviewer));

                if (epaInfo.Process.Managements.Any() && !epaInfo.Process.Managements.First().IsEndProcess)
                {
                    bodyStyle = ApplyEvenOddBodyStyle(evenRedStyle, oddRedStyle);
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(
                    EpaDetail.PhaseSelecteds,
                    score.ManagementScore.ToString(),
                    (int)ProcessType.Management));

                bodyStyle = ApplyEvenOddBodyStyle(evenStyleCenter, oddStyleCenter);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(score.DisciplineScore);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(score.FinalScore);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(score.Grade);

                _currentRow++;
            }
        }

        private ICellStyle ApplyEvenOddBodyStyle(ICellStyle evenStyle, ICellStyle oddStyle)
        {
            return _currentRow % 2 == 0 ? evenStyle : oddStyle;
        }

        private static string GetScoreByPhaseSelected(
            IEnumerable<EvaluationPhaseModel> phaseSelecteds,
            string score,
            int phaseId)
        {
            return phaseSelecteds.Count(x => x.PhaseId == phaseId).Equals(1) ? score : "-";
        }

        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification =
            "Reviewed. Suppression is OK here.")]
        private static dynamic CalculateScore(EPAModel epaInfo)
        {
            string selfScore = string.Empty;
            string evaluatorScore = string.Empty;
            string reviewerScore = string.Empty;
            string managementScore = string.Empty;
            string finalScore;
            string grade;

            var disciplineScore = epaInfo.HasDisciplineScore
                ? (epaInfo.DisciplineScore < 0
                    ? "0"
                    : epaInfo.DisciplineScore.ToString(CultureInfo.InvariantCulture))
                : (Math.Abs(epaInfo.DisciplineScore) > Constants.Tolerance
                    ? epaInfo.DisciplineScore.ToString(CultureInfo.InvariantCulture)
                    : "-");

            if (epaInfo.EPAId > 0)
            {
                selfScore = GetSelfScore(epaInfo);
                evaluatorScore = GetEvaluatorScore(epaInfo);
                reviewerScore = GetReviewerScore(epaInfo);
                managementScore = GetManagementScore(epaInfo);

                if (epaInfo.IsSemiHasMonthly)
                {
                    selfScore = epaInfo.MonthlyEPA.AverageSelfScoreConverted?.ToString() ?? Lang.NotAvailable;

                    evaluatorScore = epaInfo.MonthlyEPA.AverageEvaluatorScoreConverted?.ToString() ?? Lang.NotAvailable;
                }
            }

            if (epaInfo.Period.IsEndPeriod)
            {
                if (string.IsNullOrEmpty(epaInfo.FinalScore))
                {
                    finalScore = Lang.NotAvailable;
                    grade = Lang.NotAvailable;
                }
                else
                {
                    finalScore = Math.Round(float.Parse(epaInfo.FinalScore), 2).ToString(CultureInfo.InvariantCulture);
                    grade = epaInfo.Grade;
                }
            }
            else
            {
                finalScore = Lang.Pending;
                grade = Lang.Pending;
            }

            return new
            {
                SelfScore = selfScore,
                EvaluatorScore = evaluatorScore,
                ReviewerScore = reviewerScore,
                ManagementScore = managementScore,
                DisciplineScore = disciplineScore,
                FinalScore = finalScore,
                Grade = grade
            };
        }

        private static string GetManagementScore(EPAModel epaInfo)
        {
            string managementScore = string.Empty;

            if (epaInfo.Process.Managements.Any())
            {
                if (epaInfo.Process.Reviewers.First().StatusEndProcess == (int)ProcessStatusType.InProgress)
                {
                    managementScore = epaInfo.Process.AverageScoreByManagements?.ToString() ?? "0";
                }

                if (epaInfo.Process.Reviewers.First().StatusEndProcess == (int)ProcessStatusType.Closed)
                {
                    managementScore = epaInfo.Process.AverageScoreByManagements?.ToString() ??
                                      Lang.NotAvailable;
                }
            }

            return managementScore;
        }

        private static string GetReviewerScore(EPAModel epaInfo)
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

        private static string GetEvaluatorScore(EPAModel epaInfo)
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

        private static string GetSelfScore(EPAModel epaInfo)
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

        private ICellStyle CreateOddStyleCenter()
        {
            ICellStyle oddStyle = _workbook.CreateCellStyle();
            oddStyle.Alignment = HorizontalAlignment.Center;
            oddStyle.VerticalAlignment = VerticalAlignment.Center;

            return oddStyle;
        }

        private ICellStyle CreateOddStyleLeft()
        {
            ICellStyle oddStyle = _workbook.CreateCellStyle();
            oddStyle.Alignment = HorizontalAlignment.Left;
            oddStyle.VerticalAlignment = VerticalAlignment.Center;

            return oddStyle;
        }

        private ICellStyle CreateEvenStyleCenter()
        {
            ICellStyle evenStyle = _workbook.CreateCellStyle();
            evenStyle.Alignment = HorizontalAlignment.Center;
            evenStyle.VerticalAlignment = VerticalAlignment.Center;
            evenStyle.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            evenStyle.FillPattern = FillPattern.SolidForeground;
            evenStyle.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;

            return evenStyle;
        }

        private ICellStyle CreateEvenStyleLeft()
        {
            ICellStyle evenStyle = _workbook.CreateCellStyle();
            evenStyle.Alignment = HorizontalAlignment.Left;
            evenStyle.VerticalAlignment = VerticalAlignment.Center;
            evenStyle.FillForegroundColor = HSSFColor.LightCornflowerBlue.Index;
            evenStyle.FillPattern = FillPattern.SolidForeground;
            evenStyle.FillBackgroundColor = HSSFColor.LightCornflowerBlue.Index;

            return evenStyle;
        }

        private static IList<string> CreateColumnsName()
        {
            return new List<string>
            {
                Lang.SymbolCount,
                Lang.Code,
                Lang.FullName,
                Lang.NickName,
                Lang.Category,
                Lang.Section,
                Lang.JobTitle,
                Lang.GroupDate,
                Lang.TargetType,
                Lang.SelfScore,
                Lang.EvaluationScore,
                Lang.ReviewerScore,
                Lang.ManagementScore,
                Lang.DisciplineScore,
                Lang.FinalScore,
                Lang.Grade
            };
        }

        private void SetColumnWidth(int columnIndex, int width)
        {
            _sheet.SetColumnWidth(columnIndex, width * ExcelColumnWidth);
        }
    }
}