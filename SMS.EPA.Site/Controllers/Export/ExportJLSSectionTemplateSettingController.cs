namespace SMS.EPA.Site.Controllers.Export
{
    using System.Web.Mvc;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevelStandard.TemplateSetting;
    using SMS.EPA.Model.JobLevelStandard.TemplateSetting;
    using SMS.EPA.Services.Implement.ExportFile;
    using SMS.EPA.Services.Interfaces.ExportFile;

    public class ExportJLSSectionTemplateSettingController : Controller
    {
        private const int DefaultContentRowStartIndexForTemplate = 3;
        private const string Template = @"~\ExcelTemplates\SectionSettingTemplate.xls";

        private readonly IBaseService _baseService;
        private readonly IExportExcelService _exportExcelService;

        public ExportJLSSectionTemplateSettingController() : this(null, null)
        {
        }

        public ExportJLSSectionTemplateSettingController(IExportExcelService exportExcelService, IBaseService baseService)
        {
            _exportExcelService = exportExcelService ?? new ExportExcelService();
            _baseService = baseService ?? new BaseService();
        }
        public ActionResult ExportJLSSectionTemplateSetting(GetJobLevelStandardTemplateSettingCriteria criteria)
        {
            if (criteria != null)
            {
                var datas = _baseService.GetData<JobLevelStandardTemplateSetting>(criteria);

                _exportExcelService.FileName = "JLS_Section_Template_Setting";
                _exportExcelService.SheetName = "JLS Section Template Setting";
                _exportExcelService.Title = "JLS Section Template Setting";

                _exportExcelService.InitializeWithTemplate(Server.MapPath(Template));
                _exportExcelService.ContentRowStartIndex = DefaultContentRowStartIndexForTemplate;

                _exportExcelService.ExportWithTemplate(datas, true);
            }

            return new EmptyResult();
        }
    }
}