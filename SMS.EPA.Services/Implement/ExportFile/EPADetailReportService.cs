namespace SMS.EPA.Services.Implement
{
    using Core.Constants;
    using Core.Enumerations;
    using ExportFile;
    using Model;
    using NPOI.SS.UserModel;
    using Resources;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Web;

    public class EPADetailReportService : EPADetailReportServiceBase
    {
        public EPADetailReportService()
        {
            SheetTittle = Lang.EPADetailReportSheet;
        }

        public override IList<string> GetColumnsName => new List<string>
        {
            Lang.SymbolCount,
            Lang.Code,
            Lang.NickName,
            Lang.FullName,
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

        public override void Render(EPADetailModel epaDetail)
        {
            if (Workbook == null)
            {
                Export(epaDetail);
            }

            var response = HttpContext.Current.Response;

            response.Clear();
            response.ContentType = "application/vnd.ms-excel";

            var contentDisposition = string.IsNullOrEmpty(epaDetail.EpaYear)
                                     || string.IsNullOrEmpty(epaDetail.EpaEvaluationPeriod)
                ? $"attachment; filename={Lang.EPADetailReportName}.xls"
                : $"attachment; filename={epaDetail.EpaYear}_{epaDetail.EpaEvaluationPeriod}_{Lang.EPADetailReportName}.xls";

            response.AddHeader("Content-Disposition", contentDisposition);

            Workbook.Write(response.OutputStream);
        }

        [SuppressMessage(
            "StyleCop.CSharp.StylecopPlus.Rules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public override void WriteContentTable()
        {
            CreateDefaultStyles();
            int currentRow = 1;

            foreach (EPAModel epaInfo in EpaDetail.EpaInfos)
            {
                BodyStyle = ApplyEvenOddBodyStyle(EvenStyleCenter, OddStyleCenter);

                IRow row = Sheet.CreateRow(CurrentRow);

                int currentColumn = WriteCommonCells(epaInfo, row, currentRow++);

                BodyStyle = ApplyEvenOddBodyStyle(EvenStyleLeft, OddStyleLeft);

                ICell cell = row.CreateCell(currentColumn++);
                cell.CellStyle = BodyStyle;
                cell.SetCellValue(epaInfo.Period.TargetTypeName);

                BodyStyle = ApplyEvenOddBodyStyle(EvenStyleCenter, OddStyleCenter);

                dynamic score = CalculateScore(epaInfo);

                currentColumn = WriteScoreCells(epaInfo, currentColumn, row, score);

                BodyStyle = epaInfo.Process.Managements.Any() && epaInfo.Process.Managements.First().StatusId ==
                            (int)ProcessStatusType.InProgress
                    ? ApplyEvenOddBodyStyle(EvenRedStyle, OddRedStyle)
                    : ApplyEvenOddBodyStyle(EvenStyleCenter, OddStyleCenter);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = BodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(EpaDetail.PhaseSelecteds, score.ManagementScore.ToString(), (int)ProcessType.Management));

                BodyStyle = ApplyEvenOddBodyStyle(EvenStyleCenter, OddStyleCenter);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = BodyStyle;
                cell.SetCellValue(score.DisciplineScore);

                if (epaInfo.StatusId == (int)EPAStatus.Open)
                {
                    BodyStyle = ApplyEvenOddBodyStyle(EvenRedStyle, OddRedStyle);
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = BodyStyle;
                cell.SetCellValue(score.FinalScore);

                cell = row.CreateCell(currentColumn);
                cell.CellStyle = BodyStyle;
                cell.SetCellValue(score.Grade);

                CurrentRow++;
            }
        }

        protected override dynamic CalculateScore(EPAModel epaInfo)
        {
            string selfScore = Lang.NotAvailable;
            string evaluatorScore = Lang.NotAvailable;
            string reviewerScore = Lang.NotAvailable;
            string managementScore = Lang.NotAvailable;

            var disciplineScore = Math.Abs(epaInfo.DisciplineScore) > Constants.Tolerance ? epaInfo.DisciplineScore.ToString(CultureInfo.InvariantCulture) : "-";

            if (epaInfo.HasDisciplineScore)
            {
                disciplineScore = epaInfo.DisciplineScore < 0 ? "0" : epaInfo.DisciplineScore.ToString(CultureInfo.InvariantCulture);
            }

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

            var finalScore = epaInfo.FinalScoreToExport;
            var grade = GetGrade(epaInfo, finalScore);

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
            string managementScore = Lang.NotAvailable;

            if (epaInfo.Process.Managements.Any())
            {
                if (epaInfo.Process.Managements.First().StatusId == (int)ProcessStatusType.InProgress)
                {
                    managementScore = epaInfo.Process.AverageScoreByManagements?.ToString() ?? Lang.NotAvailable;
                }

                if (epaInfo.Process.Managements.First().StatusId == (int)ProcessStatusType.Closed)
                {
                    managementScore = epaInfo.Process.AverageScoreByManagements?.ToString() ?? Lang.NotAvailable;
                }
            }

            return managementScore;
        }
    }
}