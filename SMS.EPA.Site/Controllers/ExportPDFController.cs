namespace SMS.EPA.Site.Controllers
{
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Utilities;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class ExportPDFController : Controller
    {
        private const string _PDFFileName = @"EPA_{0}.pdf";

        public ActionResult RequestExportingEPADetail(int id)
        {
            var fileId = string.Format("{0}", id);
            string domain = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port == 60755 ? ":" + Request.Url.Port : string.Empty);
            string target = domain + "/EPA/Detail/" + id + "?exportpdf=true";
            string pdfFile = Server.MapPath(Constants.ExportingFolder + string.Format(_PDFFileName, fileId));
            new Task(() =>
            {
                var exportingPdf = new ExportingPdf(Request.Cookies, target);
                byte[] bytes = exportingPdf.Run();
                System.IO.File.WriteAllBytes(pdfFile, bytes);
            }).Start();

            return Content(fileId);
        }

        public ActionResult DownloadEPADetail(int id)
        {
            string pdfFile = Server.MapPath(Constants.ExportingFolder + string.Format(_PDFFileName, id));
            int count = 0;
            bool genereatedFile = System.IO.File.Exists(pdfFile);
            while (!genereatedFile)
            {
                count++;
                if (count == 11)
                {
                    return new EmptyResult();
                }

                Thread.Sleep(1000);
                genereatedFile = System.IO.File.Exists(pdfFile);
            }

            return File(pdfFile, "application/pdf", string.Format(_PDFFileName, id));
        }
    }
}
