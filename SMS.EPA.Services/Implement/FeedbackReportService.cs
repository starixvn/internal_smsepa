namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using OfficeOpenXml;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Report;
    using SMS.EPA.Model.Report.Feedback;
    using SMS.EPA.Repositories;

    public interface IFeedbackReportService
    {
        IEnumerable<FeedbackReport> GetData(GetFeedbackReportCriteria criteria);

        byte[] Export(IEnumerable<FeedbackReport> employees, string period);
    }

    public class FeedbackReportService : IFeedbackReportService
    {
        private readonly IFeedbackReportRepository feedbackReportRepository;
        private readonly ICryptographyService cryptographyService;

        public FeedbackReportService() : this(null, null)
        {
        }

        public FeedbackReportService(
            IFeedbackReportRepository feedbackReportRepository,
            ICryptographyService cryptographyService)
        {
            this.feedbackReportRepository = feedbackReportRepository ?? new FeedbackReportRepository();
            this.cryptographyService = cryptographyService ?? new CryptographyService();
        }

        public IEnumerable<FeedbackReport> GetData(GetFeedbackReportCriteria criteria)
        {
            IEnumerable<FeedbackReport> employees = feedbackReportRepository.GetData(criteria);

            foreach (FeedbackReport employee in employees)
            {
                foreach (FeedbackReportItem feedback in employee.Feedbacks)
                {
                    feedback.FinalScore = cryptographyService.Decrypt(feedback.FinalScore);
                }
            }

            return employees;
        }

        public byte[] Export(IEnumerable<FeedbackReport> employees, string period)
        {
            string templatePath = HttpContext.Current.Server.MapPath(@"~/ExcelTemplates/FeedbackReport.xlsx");
            using (var templatePackage = new ExcelPackage(new FileInfo(templatePath)))
            {
                using (var xlPackage = new ExcelPackage())
                {
                    const byte TotalColumns = 4;
                    const byte StartBodyRow = 4;
                    ExcelWorksheet templateWorksheet = templatePackage.Workbook.Worksheets[1];
                    ExcelWorksheet worksheet = xlPackage.Workbook.Worksheets.Add("Sheet1");
                    int rowIndex = 1;
                    int colIndex = 1;

                    // title
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);
                    worksheet.Cells[rowIndex, colIndex].Value = string.Format("360 Degree EPA Report - " + period);
                    rowIndex = StartBodyRow - 1;

                    // header
                    templateWorksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex + 1, TotalColumns]);
                    rowIndex++;

                    foreach (FeedbackReport employee in employees)
                    {
                        colIndex = 1;
                        templateWorksheet.Cells[StartBodyRow, 1, StartBodyRow, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                        worksheet.Cells[rowIndex, colIndex++].Value = employee.Nickname;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.AverageFinalScore;
                        worksheet.Cells[rowIndex, colIndex++].Value = employee.TotalComment;
                        worksheet.Cells[rowIndex, colIndex].Value = employee.TotalEvaluate;

                        rowIndex++;
                        foreach (FeedbackReportItem feedback in employee.Feedbacks)
                        {
                            colIndex = 1;
                            templateWorksheet.Cells[StartBodyRow + 1, 1, StartBodyRow + 1, TotalColumns].Copy(worksheet.Cells[rowIndex, colIndex, rowIndex, TotalColumns]);

                            worksheet.Cells[rowIndex, colIndex++].Value = employee.Nickname;
                            worksheet.Cells[rowIndex, colIndex++].Value = feedback.FinalScore;
                            worksheet.Cells[rowIndex, colIndex++].Value = feedback.Comment;
                            worksheet.Cells[rowIndex, colIndex].Value = feedback.Evaluator.Nickname;

                            rowIndex++;
                        }
                    }

                    const byte MinimumWidth = 10;
                    const byte MaximumWidth = 50;
                    worksheet.Cells.AutoFitColumns(MinimumWidth, MaximumWidth);
                    worksheet.View.FreezePanes(StartBodyRow, 1);
                    return xlPackage.GetAsByteArray();
                }
            }
        }
    }
}