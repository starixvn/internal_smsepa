namespace SMS.EPA.Services.Implement
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Utilities.NPOIExtensions;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;

    public class ExportRecommendationReportService : IExportRecommendationReportService
    {
        private const int ContentStartRowIndex = 4;
        private const string SheetName = "Recommendation Report";
        private const string FileNamePrefix = "Recommendation Report - ";
        private const string RecommendationTitlePrefix = "Recommendation - ";

        private IWorkbook Workbook { get; set; }
        private ISheet Sheet { get; set; }
        private string FileName { get; set; }
        private PeriodModel Period { get; set; }
        private IEnumerable<EPASummaryRecommendationModel> Recommendations { get; set; }

        public void InitializeWithTemplate(string templatePath)
        {
            using (var fireStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
            {
                Workbook = new HSSFWorkbook(fireStream, true);
                Sheet = Workbook.GetSheetAt(0);
            }
        }

        public void Export(EPASummaryRecommendationReportModel recommendations, string templatePath)
        {
            Period = recommendations.Period;
            Recommendations = recommendations.Recommendations;

            InitializeWithTemplate(templatePath);
            InitReportFormat();
            FillContent();

            Workbook.ToReponseStream(FileName);
        }

        private void InitReportFormat()
        {
            FileName = FileNamePrefix + Period.Name;
            Workbook.SetSheetName(0, SheetName);
            Sheet.GetRow(0).GetCell(0).SetCellValue(RecommendationTitlePrefix + Period.Name);
        }

        private void FillContent()
        {
            switch (Period.TargetType.TargetTypeId)
            {
                case (byte)TargetType.Probation:
                    FillContentProbationType();
                    break;

                case (byte)TargetType.SemiAnnual:
                    FillContentSemiTpye();
                    break;

                default:
                    return;
            }
        }

        private void FillContentProbationType()
        {
            const string RecommendationPass = "Pass";
            const string RecommendationFail = "Fail";
            const string RecommendationExtend = "Extend";
            const string StatusFormat = "{0}{1}";
            const string ActorFormat = "{0}(by {1})";

            FormatProbationTemplate();

            int rowIndex = ContentStartRowIndex;

            foreach (var recommendation in Recommendations)
            {
                int colIndex = 0;
                var curRow = Sheet.CreateRow(rowIndex);

                curRow.CreateCell(colIndex++).SetCellValue(recommendation.Code);
                curRow.CreateCell(colIndex++).SetCellValue(recommendation.Nickname);
                curRow.CreateCell(colIndex++).SetCellValue(recommendation.FullName);
                curRow.CreateCell(colIndex++).SetCellValue(recommendation.Section);
                curRow.CreateCell(colIndex++).SetCellValue(recommendation.Team);

                switch (recommendation.ProbationStatus)
                {
                    case (byte)ProbationStatusType.FailProbation:
                        curRow.CreateCell(colIndex).SetCellValue(string.Format(StatusFormat,
                            RecommendationFail,
                            !string.IsNullOrWhiteSpace(recommendation.ModifiedBy)
                                ? string.Format(ActorFormat, Environment.NewLine, recommendation.ModifiedBy) : string.Empty));
                        break;

                    case (byte)ProbationStatusType.PassProbation:
                        curRow.CreateCell(colIndex).SetCellValue(string.Format(StatusFormat,
                            RecommendationPass,
                            !string.IsNullOrWhiteSpace(recommendation.ModifiedBy)
                                ? string.Format(ActorFormat, Environment.NewLine, recommendation.ModifiedBy) : string.Empty));
                        break;

                    case (byte)ProbationStatusType.ExtendProbation:
                        curRow.CreateCell(colIndex).SetCellValue(string.Format(StatusFormat,
                            RecommendationExtend,
                            !string.IsNullOrWhiteSpace(recommendation.ModifiedBy)
                                ? string.Format(ActorFormat, Environment.NewLine, recommendation.ModifiedBy) : string.Empty));
                        break;

                    default:
                        curRow.CreateCell(colIndex).SetCellValue(string.Empty);
                        break;
                }

                rowIndex++;
            }
        }

        private void FormatProbationTemplate()
        {
            const byte JobLevelFieldColIndex = 7;
            const byte AnualIncrementFieldColIndex = 6;

            Sheet.RemoveColumn(JobLevelFieldColIndex);
            Sheet.RemoveColumn(AnualIncrementFieldColIndex);

            var recommendationCell = new CellRangeAddress(2, 3, 5, 5);
            Sheet.AddMergedRegion(recommendationCell);
        }

        private void FillContentSemiTpye()
        {
            int rowIndex = ContentStartRowIndex;
            const string StatusFormat = "{0}{1}";
            const string ActorFormat = "{0}(by {1})";

            foreach (var recommendation in Recommendations)
            {
                int colIndex = 0;
                var curRow = Sheet.CreateRow(rowIndex);

                curRow.CreateCell(colIndex++).SetCellValue(recommendation.Code);
                curRow.CreateCell(colIndex++).SetCellValue(recommendation.Nickname);
                curRow.CreateCell(colIndex++).SetCellValue(recommendation.FullName);
                curRow.CreateCell(colIndex++).SetCellValue(recommendation.Section);
                curRow.CreateCell(colIndex++).SetCellValue(recommendation.Team);
                curRow.CreateCell(colIndex++).SetCellValue(string.Format(StatusFormat,
                    recommendation.Promotion,
                    !string.IsNullOrWhiteSpace(recommendation.Promotion) && !string.IsNullOrWhiteSpace(recommendation.PromotedBy)
                        ? string.Format(ActorFormat, Environment.NewLine, recommendation.PromotedBy) : string.Empty));
                curRow.CreateCell(colIndex++).SetCellValue(string.Format(StatusFormat,
                    recommendation.AnnualIncrement,
                    !string.IsNullOrWhiteSpace(recommendation.AnnualIncrement) && !string.IsNullOrWhiteSpace(recommendation.PayIncreasedBy)
                        ? string.Format(ActorFormat, Environment.NewLine, recommendation.PayIncreasedBy) : string.Empty));
                curRow.CreateCell(colIndex).SetCellValue(string.Format(StatusFormat,
                    recommendation.JobLevel,
                    !string.IsNullOrWhiteSpace(recommendation.JobLevel) && !string.IsNullOrWhiteSpace(recommendation.JobLevelIncreasedBy)
                        ? string.Format(ActorFormat, Environment.NewLine, recommendation.JobLevelIncreasedBy) : string.Empty));

                rowIndex++;
            }
        }
    }
}