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

        public override void WriteContentTable()
        {
            CreateDefaultStyles();

            foreach (var epaInfo in EpaDetail.EpaInfos)
            {
                BodyStyle = ApplyEvenOddBodyStyle(EvenStyleCenter, OddStyleCenter);

                IRow row = Sheet.CreateRow(CurrentRow);

                var currentColumn = WriteCommonCells(epaInfo, row);

                BodyStyle = ApplyEvenOddBodyStyle(EvenStyleLeft, OddStyleLeft);

                var cell = row.CreateCell(currentColumn++);
                cell.CellStyle = BodyStyle;
                cell.SetCellValue(epaInfo.Period.TargetTypeName);

                BodyStyle = ApplyEvenOddBodyStyle(EvenStyleCenter, OddStyleCenter);

                dynamic score = CalculateScore(epaInfo);

                currentColumn = WriteScoreCells(epaInfo, currentColumn, row, score);

                if (epaInfo.Process.Managements.Any() && !epaInfo.Process.Managements.First().IsEndProcess)
                {
                    BodyStyle = ApplyEvenOddBodyStyle(EvenRedStyle, OddRedStyle);
                }

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = BodyStyle;
                cell.SetCellValue(GetScoreByPhaseSelected(
                    EpaDetail.PhaseSelecteds,
                    score.ManagementScore.ToString(),
                    (int)ProcessType.Management));

                BodyStyle = ApplyEvenOddBodyStyle(EvenStyleCenter, OddStyleCenter);

                cell = row.CreateCell(currentColumn++);
                cell.CellStyle = BodyStyle;
                cell.SetCellValue(score.DisciplineScore);

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
            string selfScore = string.Empty;
            string evaluatorScore = string.Empty;
            string reviewerScore = string.Empty;
            string managementScore = string.Empty;

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

            var finalScore = GetFinalScore(epaInfo);
            var grade = GetGrade(epaInfo);

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
                if (epaInfo.Process.Reviewers.First().StatusId == (int)ProcessStatusType.InProgress)
                {
                    managementScore = epaInfo.Process.AverageScoreByManagements?.ToString() ?? "0";
                }

                if (epaInfo.Process.Reviewers.First().StatusId == (int)ProcessStatusType.Closed)
                {
                    managementScore = epaInfo.Process.AverageScoreByManagements?.ToString() ??
                                      Lang.NotAvailable;
                }
            }

            return managementScore;
        }
    }
}