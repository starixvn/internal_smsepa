namespace SMS.EPA.Site.Controllers.Export
{
    using System.Web.Mvc;
    using SMS.EPA.Services;
    using SMS.EPA.Services.Implement.ExportFile;
    using SMS.EPA.Services.Interfaces.ExportFile;

    public class ExportJLSPeriodEmployeeSettingController : Controller
    {
        private const int DefaultContentRowStartIndexForTemplate = 3;
        private const string EmployeeSettingJLSPeriodTemplate = @"~\ExcelTemplates\EmployeeSettingJLSPeriodTemplate.xls";
        private readonly IJobLevelStandardPeriodService _jobLevelStandardPeriodService;
        private readonly IExportExcelService _exportExcelService;

        public ExportJLSPeriodEmployeeSettingController()
            : this(null, null)
        {
        }

        public ExportJLSPeriodEmployeeSettingController(
            IJobLevelStandardPeriodService jobLevelStandardPeriodService,
            IExportExcelService exportExcelService
            )
        {
            _jobLevelStandardPeriodService = jobLevelStandardPeriodService ?? new JobLevelStandardPeriodService();
            _exportExcelService = exportExcelService ?? new ExportExcelService();
        }

        public ActionResult ExportExcel(Criteria.JobLevelStandard.GetPeriodEmployeeSettingCriteria criteria)
        {
            var employees = _jobLevelStandardPeriodService.GetEmployeeSettings(criteria);

            _exportExcelService.FileName = string.Format("{0}_Employee_Setting_Job_Level_Standard_Period", criteria.PeriodName);
            _exportExcelService.SheetName = "Employee Setting Job Level Standard Period";
            _exportExcelService.Title = string.Format("{0}_Employee_Setting_Job_Level_Standard_Period", criteria.PeriodName);
            _exportExcelService.ContentRowStartIndex = DefaultContentRowStartIndexForTemplate;

            _exportExcelService.InitializeWithTemplate(Server.MapPath(EmployeeSettingJLSPeriodTemplate));

            _exportExcelService.ExportWithTemplate(employees, true, true);

            return new EmptyResult();
        }
    }
}