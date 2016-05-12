namespace SMS.EPA.Core.Utilities
{
    using Fanex.Logging;
    using SelectPdf;
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web;

    public class ExportingPdf
    {
        private const int WidthOfPdfEqual1024Screen = 1322;
        private HtmlToPdf _convert;
        private string _url;

        public ExportingPdf(HttpCookieCollection cookie, string url)
        {
            var cookies = GetUserCookiesToAuthenticate(cookie);
            _convert = new HtmlToPdf();
            _convert.Options.MinPageLoadTime = int.Parse(ConfigurationManager.AppSettings["MinPageLoadTimeForPDF"]);
            _convert.Options.WebPageWidth = WidthOfPdfEqual1024Screen;
            _convert.Options.AutoFitWidth = HtmlToPdfPageFitMode.AutoFit;
            _convert.Options.HttpCookies.Add(cookies);
            _url = url;
        }

        public byte[] Run()
        {
            PdfDocument document = _convert.ConvertUrl(_url);
            var bytes = document.Save();
            document.Close();

            return bytes;
        }

        public void WriteToFile(string filePath)
        {
            PdfDocument document = _convert.ConvertUrl(_url);
            document.Save(filePath);
            document.Close();
        }

        public void WriteToFileAsync(string filePath)
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    PdfDocument document = _convert.ConvertUrl(_url);
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
