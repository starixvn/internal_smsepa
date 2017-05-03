namespace SMS.EPA.Services.Implement
{
    using Core.Constants;
    using Core.Enumerations;
    using ExportFile;
    using Model;
    using NPOI.HSSF.Util;
    using NPOI.SS.UserModel;
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Web;

    public class MonthlyEPADetailReportService : EPADetailReportServiceBase
    {
        public MonthlyEPADetailReportService()
        {
            SheetTittle = Lang.MonthlyEPADetailReportSheet;
        }

        public override IList<string> GetColumnsName => new List<string>
        {
            Lang.SymbolCount,
            Lang.Code,
            Lang.FullName,
            Lang.NickName,
            Lang.Category,
            Lang.Section,
            Lang.JobTitle,
            Lang.GroupDate,
            Lang.SelfScore,
            Lang.EvaluationScore,
            Lang.ReviewerScore,
            Lang.FinalScore,
            Lang.Grade
        };

        public override void Render(EPADetailModel epaDetail)
        {
            if (Workbook == null)
            {
                Export(epaDetail);
            }

            var response = HttpContext.Current.Response;

            response.Clear();
            response.ContentType = "application/vnd.ms-excel";
            response.AddHeader("Content-Disposition", $"attachment; filename={Lang.MonthlyEPADetailReportName}.xls");

            Workbook.Write(response.OutputStream);
        }

        [SuppressMessage(
            "StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        [SuppressMessage(
            "Microsoft.Maintainability",
            "CA1502:AvoidExcessiveComplexity",
            Justification = "Reviewed. Suppression is OK here.")]
        public override void WriteContentTable()
        {
            IFont redFont = Workbook.CreateFont();
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

                var bodyStyle = ApplyEvenOddBodyStyle(evenStyleCenter, oddStyleCenter);

                IRow row = Sheet.CreateRow(CurrentRow);
                dynamic score = CalculateScore(epaInfo);

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

                if (!epaInfo.Process.Self.IsEndProcess)
                {
                    bodyStyle = ApplyEvenOddBodyStyle(evenRedStyle, oddRedStyle);
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(EpaDetail.PhaseSelecteds, score.SelfScore, (int)ProcessType.Self));

                if (!epaInfo.Process.Evaluators.First().IsEndProcess)
                {
                    bodyStyle = ApplyEvenOddBodyStyle(evenRedStyle, oddRedStyle);
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(EpaDetail.PhaseSelecteds, score.EvaluatorScore, (int)ProcessType.Evaluator));

                if (!epaInfo.Process.Reviewers.First().IsEndProcess)
                {
                    bodyStyle = ApplyEvenOddBodyStyle(evenRedStyle, oddRedStyle);
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(EpaDetail.PhaseSelecteds, score.ReviewerScore, (int)ProcessType.Reviewer));

                bodyStyle = ApplyEvenOddBodyStyle(evenStyleCenter, oddStyleCenter);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(score.FinalScore);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = bodyStyle;
                cell.SetCellValue(score.Grade);

                CurrentRow++;
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
        private static dynamic CalculateScore(EPAModel epaInfo)
        {
            string disciplineScore = string.Empty;
            string selfScore = string.Empty;
            string evaluatorScore = string.Empty;
            string reviewerScore = string.Empty;
            string finalScore;
            string grade;

            if (epaInfo.EPAId > 0)
            {
                selfScore = GetSelfScore(epaInfo);
                evaluatorScore = GetEvaluatorScore(epaInfo);
                reviewerScore = GetReviewerScore(epaInfo);
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
                DisciplineScore = disciplineScore,
                FinalScore = finalScore,
                Grade = grade
            };
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

        private static string GetScoreByPhaseSelected(
            IEnumerable<EvaluationPhaseModel> phaseSelecteds,
            string score,
            int phaseId)
        {
            return phaseSelecteds.Count(x => x.PhaseId == phaseId).Equals(1) ? score : "-";
        }

        private static string GetSelfScore(EPAModel epaInfo)
        {
            string selfScore = string.Empty;

            if (epaInfo.Process.Self.StatusEndProcess == (int)ProcessStatusType.InProgress)
            {
                float? score = !string.IsNullOrEmpty(epaInfo.Process.Self.TotalScore) ? float.Parse(epaInfo.Process.Self.TotalScore) : 0;
                selfScore = Math.Round(score.Value, 2).ToString(CultureInfo.InvariantCulture);
            }

            if (epaInfo.Process.Self.StatusEndProcess == (int)ProcessStatusType.Closed)
            {
                selfScore = !string.IsNullOrEmpty(epaInfo.Process.Self.TotalScore)
                    ? Math.Round(float.Parse(epaInfo.Process.Self.TotalScore), 2).ToString(CultureInfo.InvariantCulture)
                    : Lang.NotAvailable;
            }

            return selfScore;
        }
    }
}