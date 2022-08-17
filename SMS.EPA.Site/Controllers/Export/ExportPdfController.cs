namespace SMS.EPA.Site.Controllers
{
    using System.Collections.Generic;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using Core.Enumerations;
    using Core.Services;
    using Core.Services.Interfaces;
    using Core.Utilities;
    using Criteria;
    using Model;
    using Resources;
    using Services.Implement;
    using Services.Interfaces;

    public class ExportPdfController : Controller
    {
        private const string FileNameFormat = "{0}_{1}.pdf";
        private const string CssFilePath = @"~\Content\Site\pdf.css";
        private const string ViewTemplatePath = "~/Views/ExportPdf/EpaDetail.cshtml";

        private readonly IEmployeeEPAService _employeeEPAService;
        private readonly IRazorRenderService _razorRenderService;
        private readonly ISessionService _sessionService;
        private readonly IBaseService baseService;

        public ExportPdfController() : this(null, null, null, null)
        {
        }

        public ExportPdfController(
            IEmployeeEPAService employeeEPAService,
            IRazorRenderService razorRenderService,
            ISessionService sessionService,
            IBaseService baseService)
        {
            _employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            _razorRenderService = razorRenderService ?? new RazorRenderService();
            _sessionService = sessionService ?? new SessionService();
            this.baseService = baseService ?? new BaseService();
        }

        public ActionResult ExportPdf(int id)
        {
            var criteria = new GetEmployeePermissionCriteria
            {
                EPAId = id,
                ActorUserId = _sessionService.EmployeeId
            };

            int permission = baseService.GetDataEntity<int>(criteria);

            if (permission == (int)Policy.None)
            {
                return new EmptyResult();
            }

            EPAModel epaInfo = _employeeEPAService.GetEPAInfoById(id);
            _employeeEPAService.RemoveSensitiveDataOnEmployeeRole(epaInfo, _sessionService);
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
            var htmlString = _razorRenderService.RenderViewToString(this, ViewTemplatePath, epaInfo);

            return RenderPdfContent(cssFilePath, htmlString);
        }

        private string GetGradeToExport(EPAModel epaInfo)
        {
            string grade = epaInfo.Grade;

            if (epaInfo.StatusId == (byte)EpaStatus.Open)
            {
                IEnumerable<StandardGradeModel> standardGrades = baseService.GetData<StandardGradeModel>(new GetStandardGradeCriteria());
                float? performanceScore = null;
                if (float.TryParse(epaInfo.FinalScoreToExport, out float finalScore) && finalScore > 0)
                {
                    performanceScore = finalScore - epaInfo.DisciplineScore;
                }

                grade = _employeeEPAService.ConvertScoreToGrade(
                    performanceScore,
                    epaInfo.DisciplineScore,
                    epaInfo.HasDisciplineScore,
                    standardGrades);
            }
            else
            {
                if (string.IsNullOrEmpty(epaInfo.FinalScore))
                {
                    grade = Lang.NotAvailable;
                }
            }

            return grade;
        }
    }
}