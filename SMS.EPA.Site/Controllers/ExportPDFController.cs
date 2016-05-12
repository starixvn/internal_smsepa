namespace SMS.EPA.Site.Controllers
{
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Criteria.EmployeeEPA;
    using SMS.EPA.Model.EmployeeEPA;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;
    using System;
    using System.Configuration;
    using System.Web.Mvc;

    public class ExportPDFController : Controller
    {
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

        public string RequestExportEPA(int epaId)
        {
            string target = "/EPA/Detail/" + epaId + "?exportpdf=true";
            string fileName = RequestExport(target);

            return fileName;
        }

        public string RequestExportSemiEPA(int epaId)
        {
            string target = "/EPA/SemiDetail/" + epaId + "?exportpdf=true";
            string fileName = RequestExport(target);

            return fileName;
        }

        public ActionResult DownloadEPADetail(int epaId, string fileName)
        {
            string pdfFile = Server.MapPath(Constants.ExportingFolder + fileName);
            bool isValid = FileUtilities.IsReadyWithIntervals(pdfFile, int.Parse(ConfigurationManager.AppSettings["IntervalToDownloadFile"]));
            if (!isValid)
            {
                return HttpNotFound();
            }

            byte[] bytes = FileUtilities.ReadAndDeleteFile(pdfFile);
            string newFileName = FormatPdfFileName(epaId);

            return File(bytes, "application/pdf", newFileName);
        }

        private string FormatPdfFileName(int epaId)
        {
            var criteria = new GetEPAInfoByIdCriteria() { EPAId = epaId };

            EPAModel epaInfo = _employeeEPAService.GetEPAInfoById(criteria);
            string code = string.IsNullOrEmpty(epaInfo.Employee.Code) ? string.Empty : "_" + epaInfo.Employee.Code;

            return string.Format("{0}.pdf", epaInfo.Employee.NickName + epaInfo.Employee.Code);
        }

        private string RequestExport(string target)
        {
            string fileName = string.Format("EPA_{0}.pdf", DateTime.Now.ToString("HHmmss"));
            string domain = string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Host, Request.Url.Port != 0 ? ":" + Request.Url.Port : string.Empty);
            string url = domain + target;
            string pdfFile = Server.MapPath(Constants.ExportingFolder + fileName);

            var exportingPdf = new ExportingPdf(Request.Cookies, url);
            exportingPdf.WriteToFileAsync(pdfFile);

            return fileName;
        }
    }
}
