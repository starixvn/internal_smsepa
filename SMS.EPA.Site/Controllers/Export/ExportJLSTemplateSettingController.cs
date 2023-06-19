namespace SMS.EPA.Site.Controllers.Export
{
    using System.Web.Mvc;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevelStandard.TemplateSetting;
    using SMS.EPA.Model.JobLevelStandard.TemplateSetting;
    using SMS.EPA.Services.Implement.ExportFile;
    using SMS.EPA.Services.Interfaces.ExportFile;

    public class ExportJLSTemplateSettingController : Controller
    {
        private const int DefaultContentRowStartIndexForTemplate = 3;
        private const string Template = @"~\ExcelTemplates\JLSSectionTemplateSetting.xls";

        private readonly IBaseService _baseService;
        private readonly IExportExcelService _exportExcelService;

        public ExportJLSTemplateSettingController() : this(null, null)
        {
        }

        public ExportJLSTemplateSettingController(IExportExcelService exportExcelService, IBaseService baseService)
        {
            _exportExcelService = exportExcelService ?? new ExportExcelService();
            _baseService = baseService ?? new BaseService();
        }

        public ActionResult ExportSectionTemplateSetting(GetJobLevelStandardTemplateSettingCriteria criteria)
        {
            var datas = _baseService.GetData<JobLevelStandardTemplateSetting>(criteria);

            _exportExcelService.FileName = "JLS_Section_Template_Setting";
            _exportExcelService.SheetName = "Section";
            _exportExcelService.Title = "JLS Template Setting";

            _exportExcelService.InitializeWithTemplate(Server.MapPath(Template));
            _exportExcelService.ContentRowStartIndex = DefaultContentRowStartIndexForTemplate;

            _exportExcelService.ExportWithTemplate(datas, true, true);

            return new EmptyResult();
        }
    }
}