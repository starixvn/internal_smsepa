namespace SMS.EPA.Site.Controllers
{
    using Core.Enumerations;
    using Core.Services;
    using Core.Services.Interfaces;
    using Core.Utilities;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Hosting;
    using System.Web.Mvc;

    public class ExportPdfController : Controller
    {
        private const string FileNameFormat = "{0}_{1}.pdf";
        private const string CssFilePath = @"~\Content\Site\pdf.css";
        private const string ViewTemplatePath = "~/Views/ExportPdf/EpaDetail.cshtml";

        private readonly IEmployeeEPAService employeeEPAService;
        private readonly IRazorRenderService razorRenderService;

        public ExportPdfController()
            : this(null, null)
        {
        }

        public ExportPdfController(
            IEmployeeEPAService employeeEPAService,
            IRazorRenderService razorRenderService)
        {
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.razorRenderService = razorRenderService ?? new RazorRenderService();
        }

        public ActionResult EpaDetail(int id)
        {
            EPAModel epaInfo = employeeEPAService.GetEPAInfoById(id);
            epaInfo.Grade = GetGradeToExport(epaInfo);

            return View(epaInfo);
        }

        public ActionResult ExportPdf(int id)
        {
            EPAModel epaInfo = employeeEPAService.GetEPAInfoById(id);
            epaInfo.Grade = GetGradeToExport(epaInfo);

            byte[] streamContent = RenderPdfContent(epaInfo);
            var fileName = string.Format(FileNameFormat, epaInfo.Employee.Code, epaInfo.Employee.NickName);

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

        private byte[] RenderPdfContent(EPAModel epaInfo)
        {
            var cssFilePath = GetPhysicalFilePath(CssFilePath);
            var htmlString = razorRenderService.RenderViewToString(this, ViewTemplatePath, epaInfo);

            return RenderPdfContent(cssFilePath, htmlString);
        }

        private string GetGradeToExport(EPAModel epaInfo)
        {
            string grade = epaInfo.Grade;

            if (epaInfo.StatusId == (int)EPAStatus.Open)
            {
                IEnumerable<StandardGradeModel> standardGrades = employeeEPAService.GetStandardGrade();
                float finalScore;
                float.TryParse(epaInfo.FinalScoreToExport, out finalScore);

                float? performanceScore = null;
                if (finalScore > 0)
                {
                    performanceScore = finalScore - epaInfo.DisciplineScore;
                }

                grade = employeeEPAService.ConvertScoreToGrade(
                    performanceScore,
                    epaInfo.DisciplineScore,
                    epaInfo.HasDisciplineScore,
                    standardGrades);
            }

            return grade;
        }
    }
}