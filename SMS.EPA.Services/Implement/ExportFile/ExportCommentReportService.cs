namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;
    using NPOI.SS.Util;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Utilities.NPOIExtensions;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Interfaces;

    public class ExportCommentReportService : IExportCommentReportService
    {
        private const string FileNamePrefix = "Comment Report - ";
        private const string TitleProfix = "Comment - ";
        private const int TableContentFirstRowIndex = 4;
        private const int TotalStaffInfoField = 5;
        private const int EvaluatorColumnIndex = 5;
        private const int ReviewerColumnIndex = 7;
        private const int ManagementColumnIndex = 9;

        private IWorkbook Workbook { get; set; }
        private ISheet Sheet { get; set; }
        private string FileName { get; set; }
        private string Title { get; set; }
        private IEnumerable<EPASummaryCommentModel> SummaryComments { get; set; }

        public void Export(EPASummaryCommentReportModel data, string templatePath)
        {
            InitializeWithTemplate(templatePath);
            GetData(data);

            FillContent();

            Workbook.ToReponseStream(FileName);
        }

        private void InitializeWithTemplate(string templatePath)
        {
            try
            {
                using (var fireStream = new FileStream(templatePath, FileMode.Open, FileAccess.Read))
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

        private void GetData(EPASummaryCommentReportModel data)
        {
            SummaryComments = data.CommentSummary;
            Title = TitleProfix + data.PeriodName;
            FileName = FileNamePrefix + data.PeriodName;
        }

        private void FillContent()
        {
            var titleCell = Sheet.GetRow(0).GetCell(0);
            titleCell.SetCellValue(Title);

            var staffFields = SummaryComments.GroupBy(comment => new { comment.Code, comment.Nickname, comment.Section, comment.Team, comment.StaffComment });
            int rowIndex = TableContentFirstRowIndex;

            foreach (var staffField in staffFields)
            {
                int colIndex = 0;
                var evaluatorRow = new Dictionary<string, int>
                {
                    { "Evaluator", rowIndex },
                    { "Reviewer", rowIndex },
                    { "Management", rowIndex }
                };

                var curRow = Sheet.CreateRow(rowIndex);
                var fieldData = staffField.Key;

                curRow.CreateCell(colIndex++).SetCellValue(fieldData.Code);
                curRow.CreateCell(colIndex++).SetCellValue(fieldData.Nickname);
                curRow.CreateCell(colIndex++).SetCellValue(fieldData.Section);
                curRow.CreateCell(colIndex++).SetCellValue(fieldData.Team);
                curRow.CreateCell(colIndex).SetCellValue(fieldData.StaffComment);

                FillEvaluatorFields(evaluatorRow, fieldData.Code);

                int nextStaffRowIndex = evaluatorRow.Values.Max();
                MergeStaffFields(rowIndex, nextStaffRowIndex - 1);

                rowIndex = nextStaffRowIndex;
            }
        }

        private void FillEvaluatorFields(Dictionary<string, int> evaluatorRow, string staffCode)
        {
            var evaluatorComments = SummaryComments.Where(comment => comment.Code == staffCode);

            foreach (var evaluatorComment in evaluatorComments)
            {
                switch (evaluatorComment.ProcessId)
                {
                    case (byte)ProcessType.Evaluator:
                        int evaluatorRowIndex = evaluatorRow["Evaluator"];
                        GetExcelRow(evaluatorRowIndex).CreateCell(EvaluatorColumnIndex).SetCellValue(evaluatorComment.EvaluatorNickName);
                        GetExcelRow(evaluatorRowIndex).CreateCell(EvaluatorColumnIndex + 1).SetCellValue(evaluatorComment.EvaluatorComment);
                        evaluatorRow["Evaluator"] = evaluatorRowIndex + 1;
                        break;

                    case (byte)ProcessType.Reviewer:
                        int reviewerRowIndex = evaluatorRow["Reviewer"];
                        GetExcelRow(reviewerRowIndex).CreateCell(ReviewerColumnIndex).SetCellValue(evaluatorComment.EvaluatorNickName);
                        GetExcelRow(reviewerRowIndex).CreateCell(ReviewerColumnIndex + 1).SetCellValue(evaluatorComment.EvaluatorComment);
                        evaluatorRow["Reviewer"] = reviewerRowIndex + 1;
                        break;

                    case (byte)ProcessType.Management:
                        int managementRowIndex = evaluatorRow["Management"];
                        GetExcelRow(managementRowIndex).CreateCell(ManagementColumnIndex).SetCellValue(evaluatorComment.EvaluatorNickName);
                        GetExcelRow(managementRowIndex).CreateCell(ManagementColumnIndex + 1).SetCellValue(evaluatorComment.EvaluatorComment);
                        evaluatorRow["Management"] = managementRowIndex + 1;
                        break;

                    default:
                        continue;
                }
            }
        }

        private void MergeStaffFields(int startRowIndex, int lasRowIndex)
        {
            for (int colIndex = 0; colIndex < TotalStaffInfoField; colIndex++)
            {
                var mergeRegion = new CellRangeAddress(startRowIndex, lasRowIndex, colIndex, colIndex);
                Sheet.AddMergedRegion(mergeRegion);
            }
        }

        private IRow GetExcelRow(int rowIndex)
        {
            return Sheet.GetRow(rowIndex) ?? Sheet.CreateRow(rowIndex);
        }
    }
}