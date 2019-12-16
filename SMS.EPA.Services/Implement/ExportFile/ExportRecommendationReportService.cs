namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Diagnostics;
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

        private IWorkbook Workbook { get; set; }
        private ISheet Sheet { get; set; }
        private string FileName { get; set; }
        private PeriodModel Period { get; set; }
        private IEnumerable<EPASummaryRecommendationModel> Recommendations { get; set; }

        public void InitializeWithTemplate(string templatePath)
        {
            try
            {
                using (FileStream fireStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
                {
                    Workbook = new HSSFWorkbook(fireStream, true);
                    Sheet = Workbook.GetSheetAt(0);
                }
            }
            catch (IOException exception)
            {
                Debug.WriteLine(exception.Message);
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
            Sheet.GetRow(0).GetCell(0).SetCellValue(Period.Name);
        }

        private void FillContent()
        {
            switch (Period.TargetType.TargetTypeId)
            {
                case (int)TargetType.Probation:
                    FillContentProbationType();
                    break;

                case (int)TargetType.SemiAnnual:
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

            FormatProbationTemplate();

            int rowIndex = ContentStartRowIndex;

            foreach (var recommendation in Recommendations)
            {
                int colIndex = 0;
                var curRow = Sheet.CreateRow(rowIndex);

                curRow.CreateCell(colIndex++).SetCellValue(recommendation.Code);
                curRow.CreateCell(colIndex++).SetCellValue(recommendation.NickName);
                curRow.CreateCell(colIndex++).SetCellValue(recommendation.Section);

                switch (recommendation.ProbationStatus)
                {
                    case (byte)ProbationStatusType.FailProbation:
                        curRow.CreateCell(colIndex).SetCellValue(RecommendationFail);
                        break;

                    case (byte)ProbationStatusType.PassProbation:
                        curRow.CreateCell(colIndex).SetCellValue(RecommendationPass);
                        break;

                    case (byte)ProbationStatusType.ExtendProbation:
                        curRow.CreateCell(colIndex).SetCellValue(RecommendationExtend);
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
            const int jobGradeFieldColIndex = 5;
            const int anualIncrementFieldColIndex = 4;

            Sheet.RemoveColumn(jobGradeFieldColIndex);
            Sheet.RemoveColumn(anualIncrementFieldColIndex);

            var recommendationCell = new CellRangeAddress(2, 3, 3, 3);
            Sheet.AddMergedRegion(recommendationCell);
        }

        private void FillContentSemiTpye()
        {
            int rowIndex = ContentStartRowIndex;

            foreach (var recommendation in Recommendations)
            {
                int colIndex = 0;
                var curRow = Sheet.CreateRow(rowIndex);

                curRow.CreateCell(colIndex++).SetCellValue(recommendation.Code);
                curRow.CreateCell(colIndex++).SetCellValue(recommendation.NickName);
                curRow.CreateCell(colIndex++).SetCellValue(recommendation.Section);
                curRow.CreateCell(colIndex++).SetCellValue(recommendation.Promotion);
                curRow.CreateCell(colIndex++).SetCellValue(recommendation.AnnualIncrement);
                curRow.CreateCell(colIndex).SetCellValue(recommendation.JobGrade);

                rowIndex++;
            }
        }
    }
}