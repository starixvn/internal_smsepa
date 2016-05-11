namespace SMS.EPA.Core.Utilities
{
    using SelectPdf;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Web;

    public class ExportingPdf
    {
        private HtmlToPdf _convert;
        private string _url;

        public ExportingPdf(HttpCookieCollection cookie, string url)
        {
            var cookies = GetUserCookiesToAuthenticate(cookie);
            _convert = new HtmlToPdf();
            _convert.Options.MinPageLoadTime = int.Parse(ConfigurationManager.AppSettings["MinPageLoadTimeForPDF"]);
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
