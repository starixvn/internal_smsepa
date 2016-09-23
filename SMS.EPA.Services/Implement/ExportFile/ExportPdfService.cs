namespace SMS.EPA.Core.Utilities
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web;
    using Fanex.Logging;
    using SelectPdf;
    using SMS.EPA.Criteria;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class ExportPdfService : IExportPdfService
    {
        private const int NormalWidthPdfPage = 1322;
        private const int LargestWidthPdfPage = 1920;

        private readonly IEmployeeEPAService _epaService;
        private HtmlToPdf _convert;

        public ExportPdfService()
            : this(null)
        {
        }

        public ExportPdfService(IEmployeeEPAService epaService)
        {
            _epaService = epaService ?? new EmployeeEPAService();
            _convert = new HtmlToPdf();
            _convert.Options.MinPageLoadTime = int.Parse(ConfigurationManager.AppSettings["MinPageLoadTimeForPDF"]);
        }

        public void WriteToFileAsync(string filePath, string convertedUrl, HttpCookieCollection cookie)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    var cookies = GetUserCookiesToAuthenticate(cookie);
                    _convert.Options.HttpCookies.Add(cookies);
                    PdfDocument document = _convert.ConvertUrl(convertedUrl);
                    document.Save(filePath);
                    document.Close();
                }
                catch (Exception ex)
                {
                    string message = ex.Message +
                       "\r\nSource: " + ex.Source +
                       "\r\nTargetSite: " + ex.TargetSite +
                       "\r\nStackTrace: " + ex.StackTrace;

                    LogService.Error(message);
                }
            });
        }

        public void WriteEPADetailToFileAsync(int epaId, string filePath, string epaDetailUrl, HttpCookieCollection cookie)
        {
            var isNormalMode = DisplayOnNormalWidthPage(epaId);
            _convert.Options.WebPageWidth = isNormalMode ? NormalWidthPdfPage : LargestWidthPdfPage;

            WriteToFileAsync(filePath, epaDetailUrl, cookie);
        }

        public bool DisplayOnNormalWidthPage(int epaId)
        {
            var rowOfEpaDetail = GetNumberOfRow(epaId);
            var maxRowOfNormalPage = int.Parse(ConfigurationManager.AppSettings["MaxNumberRowsInPdfPage"]);

            return maxRowOfNormalPage > rowOfEpaDetail;
        }

        public int GetNumberOfRow(int epaId)
        {
            var criteria = new GetNumberOfRowCriteria { EPAId = epaId };

            return _epaService.GetNumberOfRow(criteria);
        }

        private NameValueCollection GetUserCookiesToAuthenticate(HttpCookieCollection cookie)
        {
            var cookies = new NameValueCollection();
            foreach (var item in cookie.AllKeys)
            {
                cookies.Add(item, cookie[item].Value);
            }

            return cookies;
        }
    }
}