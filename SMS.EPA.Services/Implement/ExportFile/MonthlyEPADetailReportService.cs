namespace SMS.EPA.Services.Implement
{
    using ExportFile;
    using Model;
    using NPOI.SS.UserModel;
    using Resources;
    using System.Collections.Generic;
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

        public override void WriteContentTable()
        {
            CreateDefaultStyles();

            foreach (var epaInfo in EpaDetail.EpaInfos)
            {
                BodyStyle = ApplyEvenOddBodyStyle(EvenStyleCenter, OddStyleCenter);

                IRow row = Sheet.CreateRow(CurrentRow);

                var currentColumn = WriteCommonCells(epaInfo, row);

                dynamic score = CalculateScore(epaInfo);

                currentColumn = WriteScoreCells(epaInfo, currentColumn, row, score);

                BodyStyle = ApplyEvenOddBodyStyle(EvenStyleCenter, OddStyleCenter);

                var cell = row.CreateCell(currentColumn++);
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
            string disciplineScore = string.Empty;
            string selfScore = string.Empty;
            string evaluatorScore = string.Empty;
            string reviewerScore = string.Empty;

            if (epaInfo.EPAId > 0)
            {
                selfScore = GetSelfScore(epaInfo);
                evaluatorScore = GetEvaluatorScore(epaInfo);
                reviewerScore = GetReviewerScore(epaInfo);
            }

            var finalScore = GetFinalScore(epaInfo);
            var grade = GetGrade(epaInfo);

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
    }
}