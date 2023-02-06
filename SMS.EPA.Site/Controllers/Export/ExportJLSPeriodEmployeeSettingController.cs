namespace SMS.EPA.Site.Controllers.Export
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using SMS.EPA.Model.JobLevelStandard;
    using SMS.EPA.Services;
    using SMS.EPA.Services.Implement.ExportFile;
    using SMS.EPA.Services.Interfaces.ExportFile;

    public class ExportJLSPeriodEmployeeSettingController : Controller
    {
        private const int DefaultContentRowStartIndexForTemplate = 3;
        private const string EmployeeSettingJLSPeriodTemplate = @"~\ExcelTemplates\EmployeeSettingJLSPeriodTemplate.xls";
        private readonly IJobLevelStandardPeriodService periodService;
        private readonly IExportExcelService exportExcelService;

        public ExportJLSPeriodEmployeeSettingController()
            : this(null, null)
        {
        }

        public ExportJLSPeriodEmployeeSettingController(
            IJobLevelStandardPeriodService periodService,
            IExportExcelService exportExcelService)
        {
            this.periodService = periodService ?? new JobLevelStandardPeriodService();
            this.exportExcelService = exportExcelService ?? new ExportExcelService();
        }

        public ActionResult ExportExcel(Criteria.JobLevelStandard.GetPeriodEmployeeSettingCriteria criteria)
        {
            IList<PeriodEmployeeSetting> employees = periodService.GetEmployeeSettings(criteria);

            exportExcelService.FileName = string.Format("{0}_Employee_Setting", criteria.PeriodName);
            exportExcelService.SheetName = "Employee Setting";
            exportExcelService.Title = string.Format("{0}- Employee Setting", criteria.PeriodName);
            exportExcelService.ContentRowStartIndex = DefaultContentRowStartIndexForTemplate;

            exportExcelService.InitializeWithTemplate(Server.MapPath(EmployeeSettingJLSPeriodTemplate));

            exportExcelService.ExportWithTemplate(employees, true, true);

            return new EmptyResult();
        }
    }
}