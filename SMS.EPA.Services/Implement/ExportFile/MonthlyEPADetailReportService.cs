namespace SMS.EPA.Services.Implement
{
    using ExportFile;
    using Model;
    using NPOI.SS.UserModel;
    using Resources;
    using System.Collections.Generic;
    using System.Web;
    using Core.Enumerations;

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
            Lang.NickName,
            Lang.FullName,
            Lang.Category,
            Lang.Section,
            Lang.JobTitle,
            Lang.GroupDate,
            Lang.TargetTemplate,
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
            int currentRow = 1;

            foreach (EPAModel epaInfo in EpaDetail.EpaInfos)
            {
                BodyStyle = ApplyEvenOddBodyStyle(EvenStyleCenter, OddStyleCenter);

                IRow row = Sheet.CreateRow(CurrentRow);

                var currentColumn = WriteCommonCells(epaInfo, row, currentRow++);

                BodyStyle = ApplyEvenOddBodyStyle(EvenStyleLeft, OddStyleLeft);

                ICell cell = row.CreateCell(currentColumn++);
                cell.CellStyle = BodyStyle;
                cell.SetCellValue(epaInfo.Period.TargetTemplate);
                dynamic score = CalculateScore(epaInfo);

                currentColumn = WriteScoreCells(epaInfo, currentColumn, row, score);
                BodyStyle = ApplyEvenOddBodyStyle(EvenStyleLeft, OddStyleLeft);
                BodyStyle = epaInfo.StatusId == (int)EPAStatus.Open
                    ? ApplyEvenOddBodyStyle(EvenRedStyle, OddRedStyle)
                    : ApplyEvenOddBodyStyle(EvenStyleCenter, OddStyleCenter);

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
            string disciplineScore = Lang.NotAvailable;
            string selfScore = Lang.NotAvailable;
            string evaluatorScore = Lang.NotAvailable;
            string reviewerScore = Lang.NotAvailable;

            if (epaInfo.EPAId > 0)
            {
                selfScore = GetSelfScore(epaInfo);
                evaluatorScore = GetEvaluatorScore(epaInfo);
                reviewerScore = GetReviewerScore(epaInfo);
            }

            var finalScore = epaInfo.FinalScoreToExport;
            var grade = GetGrade(epaInfo, finalScore);

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