namespace SMS.EPA.Site.Controllers
{
    using Core.Services;
    using Core.Services.Interfaces;
    using Core.Utilities;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System.Web.Hosting;
    using System.Web.Mvc;

    public class ExportPdfNewController : Controller
    {
        private const string FileNameFormat = "{0}_{1}.pdf";
        private const string CssFilePath = @"~\Content\Site\pdf.css";
        private const string ViewTemplatePath = "~/Views/ExportPdf/EpaDetail.cshtml";

        private readonly IEmployeeEPAService employeeEPAService;
        private readonly IRazorRenderService razorRenderService;

        public ExportPdfNewController()
            : this(null, null)
        {
        }

        public ExportPdfNewController(
            IEmployeeEPAService employeeEPAService,
            IRazorRenderService razorRenderService)
        {
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.razorRenderService = razorRenderService ?? new RazorRenderService();
        }

        public ActionResult EpaDetail(int id)
        {
            var epaInfo = new EpaDetailModel
            {
                EpaInfo = employeeEPAService.GetEPAInfoById(id),
                Grades = employeeEPAService.GetStandardGrade()
            };

            return View(epaInfo);
        }

        public ActionResult ExportPdf(int id)
        {
            var epaInfo = new EpaDetailModel
            {
                EpaInfo = employeeEPAService.GetEPAInfoById(id)
            };

            var streamContent = RenderPdfContent(epaInfo);
            var fileName = string.Format(FileNameFormat, epaInfo.EpaInfo.Employee.Code, epaInfo.EpaInfo.Employee.NickName);

            return File(streamContent, @"application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml", fileName);
        }

        private static string GetPhysicalFilePath(string virtualFilePath)
        {
            return HostingEnvironment.MapPath(virtualFilePath);
        }

        private static byte[] RenderPdfContent(string cssFilePath, string htmlString)
        {
            return PdfRender.ContentRender(cssFilePath, htmlString);
        }

        private byte[] RenderPdfContent(EpaDetailModel epaInfo)
        {
            var cssFilePath = GetPhysicalFilePath(CssFilePath);
            var htmlString = razorRenderService.RenderViewToString(this, ViewTemplatePath, epaInfo);

            return RenderPdfContent(cssFilePath, htmlString);
        }
    }
}