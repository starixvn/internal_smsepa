namespace SMS.EPA.Site.Controllers
{
    using SelectPdf;
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class ExportPDFController : Controller
    {
        private const string _TempFolder = @"~\ExportPDFFolder\";
        private const string _PDFFileName = @"EPA_{0}.pdf";

        public ActionResult RequestExportingEPADetail(int id)
        {
            var cookies = GetUserCookiesToAuthenticate();

            var converter = new HtmlToPdf();
            converter.Options.MinPageLoadTime = int.Parse(ConfigurationManager.AppSettings["MinPageLoadTimeForPDF"]);
            converter.Options.HttpCookies.Add(cookies);

            var exportTime = DateTime.Now.ToString("yyyyMMdd-HHmmss");
            var fileId = string.Format("{0}", id);
            Task.Run(() =>
            {
                var domain = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port == 60755 ? ":" + Request.Url.Port : string.Empty);

                PdfDocument document = converter.ConvertUrl(domain + "/EPA/Detail/" + id + "?exportpdf=true");
                string pdfFile = Server.MapPath(_TempFolder + string.Format(_PDFFileName, fileId));
                document.Save(pdfFile);
                document.Close();
            });

            return Content(fileId);
        }

        public ActionResult DownloadEPADetail(int id)
        {
            string pdfFile = Server.MapPath(_TempFolder + string.Format(_PDFFileName, id));

            return File(pdfFile, "application/pdf");
        }

        private NameValueCollection GetUserCookiesToAuthenticate()
        {
            var userCookies = Request.Cookies;
            var cookies = new NameValueCollection();
            foreach (var item in userCookies.AllKeys)
            {
                cookies.Add(item, userCookies[item].Value);
            }

            return cookies;
        }
    }
}
