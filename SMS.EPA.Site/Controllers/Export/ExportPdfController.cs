namespace SMS.EPA.Site.Controllers
{
    using System.Collections.Generic;
    using System.Web.Hosting;
    using System.Web.Mvc;
    using HRM.UserControl.Client.AspNetMvc.Session;
    using HRM.UserControl.Client.Core.Enumerables;
    using HRM.UserControl.Client.Core.Models.Role;
    using HRM.UserControl.Client.Core.Services;
    using HRM.UserControl.Client.Core.Session;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Core.Utilities;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Resources;
    using SMS.EPA.Services;

    public class ExportPdfController : Controller
    {
        private const string FileNameFormat = "{0}_{1}.pdf";
        private const string CssFilePath = @"~\Content\Site\pdf.css";
        private const string ViewTemplatePath = "~/Views/ExportPdf/EpaDetail.cshtml";

        private readonly IEmployeeEPAService employeeEPAService;
        private readonly IRazorRenderService razorRenderService;
        private readonly IUserControlSession userControlSession;
        private readonly IRoleService roleService;
        private readonly IBaseService baseService;

        public ExportPdfController() : this(null, null, null, null, null)
        {
        }

        public ExportPdfController(
            IEmployeeEPAService employeeEPAService,
            IRazorRenderService razorRenderService,
            IUserControlSession userControlSession,
            IRoleService roleService,
            IBaseService baseService)
        {
            this.employeeEPAService = employeeEPAService ?? new EmployeeEPAService();
            this.razorRenderService = razorRenderService ?? new RazorRenderService();
            this.userControlSession = userControlSession ?? new UserControlSession();
            this.roleService = roleService ?? new RoleService();
            this.baseService = baseService ?? new BaseService();
        }

        public ActionResult ExportPdf(int id)
        {
            int currentEmployeeId = userControlSession.Get<int>(Constants.CurrentEmployeeIdKey);
            var criteria = new GetEmployeePermissionCriteria
            {
                EPAId = id,
                ActorUserId = currentEmployeeId
            };

            int permission = baseService.GetDataEntity<int>(criteria);

            if (permission == (byte)Policy.None)
            {
                return new EmptyResult();
            }

            EPAModel epaInfo = employeeEPAService.GetEPAInfoById(id);
            IList<Role> roles = roleService.GetBySiteId(userControlSession.SiteId);
            employeeEPAService.RemoveSensitiveDataOnEmployeeRole(epaInfo, currentEmployeeId, roles);
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

            if (epaInfo.StatusId == (byte)EpaStatus.Open)
            {
                var standardGrades = baseService.GetData<StandardGradeModel>(new GetStandardGradeCriteria());
                float? performanceScore = null;
                if (float.TryParse(epaInfo.FinalScoreToExport, out float finalScore) && finalScore > 0)
                {
                    performanceScore = finalScore - epaInfo.DisciplineScore;
                }

                grade = employeeEPAService.ConvertScoreToGrade(
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