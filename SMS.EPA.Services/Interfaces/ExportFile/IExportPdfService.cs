namespace SMS.EPA.Services.Interfaces
{
    using System.Web;

    public interface IExportPdfService
    {
        void WriteToFileAsync(string filePath, string convertedUrl, HttpCookieCollection cookie);

        void WriteEPADetailToFileAsync(int epaId, string filePath, string epaDetailUrl, HttpCookieCollection cookie);

        int GetNumberOfRow(int epaId);

        bool DisplayOnNormalWidthPage(int epaId);
    }
}
