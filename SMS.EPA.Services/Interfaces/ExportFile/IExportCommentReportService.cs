namespace SMS.EPA.Services.Interfaces
{
    using SMS.EPA.Model;

    public interface IExportCommentReportService
    {
        void Export(EPASummaryCommentReportModel datas, string templatePath);
    }
}