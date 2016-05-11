namespace SMS.EPA.Site.Controllers
{
    using Fanex.Logging;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web.Mvc;

    public class ExportPDFController : Controller
    {
        private const string _PDFFileName = @"{0}.pdf";
        private readonly IEmployeeEPAService _employeeEPAService;

        public ExportPDFController()
            : this(null)
        {
        }

        public ExportPDFController(
            IEmployeeEPAService employeeEPAService)
        {
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
        }

        public ActionResult RequestExportingEPADetail(int id)
        {
            var fileId = string.Format("{0}", id);
            string domain = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port != 0 ? ":" + Request.Url.Port : string.Empty);
            string target = domain + "/EPA/Detail/" + id + "?exportpdf=true";
            string pdfFile = Server.MapPath(Constants.ExportingFolder + FormatPdfFile(id));

            Task.Factory.StartNew(() =>
            {
                try
                {
                    var exportingPdf = new ExportingPdf(Request.Cookies, target);
                    exportingPdf.WriteToFile(pdfFile);
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message);
                }
            });

            return new EmptyResult();
        }

        public ActionResult DownloadEPADetail(int id)
        {
            string pdfFileName = FormatPdfFile(id);
            string pdfFile = Server.MapPath(Constants.ExportingFolder + pdfFileName);

            bool isExisted = IsExistsFile(pdfFile);
            if (isExisted)
            {
                byte[] bytes = ReadAndDeleteFile(pdfFile);

                return File(bytes, "application/pdf", pdfFileName);
            }

            return HttpNotFound();
        }

        private string FormatPdfFile(int epaId)
        {
            var _PdfFileFormat = @"{0}.pdf";
            var criteria = new GetEPAInfoByIdCriteria() { EPAId = epaId };

            EPAModel epaInfo = _employeeEPAService.GetEPAInfoById(criteria);
            string code = string.IsNullOrEmpty(epaInfo.Employee.Code) ? string.Empty : "_" + epaInfo.Employee.Code;

            return string.Format(_PdfFileFormat, epaInfo.Employee.NickName + epaInfo.Employee.Code);
        }

        private bool IsExistsFile(string pdfFile)
        {
            bool genereatedFile = System.IO.File.Exists(pdfFile);
            int count = 0;
            while (!genereatedFile && count < 20)
            {
                count++;

                Thread.Sleep(1000);
                genereatedFile = System.IO.File.Exists(pdfFile);
            }

            return genereatedFile && count < 20;
        }

        private byte[] ReadAndDeleteFile(string pdfFile)
        {
            var file = System.IO.File.ReadAllBytes(pdfFile);
            System.IO.File.Delete(pdfFile);

            return file;
        }
    }
}
