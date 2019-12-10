namespace SMS.EPA.Site.Controllers.Export
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class ExportTargetTemplateController : Controller
    {
        private const string TargetTemplateKPIsTemplate = @"~\ExcelTemplates\TargetTemplateKPIsTemplate.xls";

        private readonly IBaseService _baseService;
        private readonly IExportTargetTemplateService _exportTargetTemplateService;
        private readonly ITargetTemplateService _targetTemplateService;

        public ExportTargetTemplateController()
            : this(null, null, null)
        {
        }

        public ExportTargetTemplateController(

            IExportTargetTemplateService exportTargetTemplateService,
            ITargetTemplateService targetTemplateService,
            IBaseService baseService
            )
        {
            _baseService = baseService ?? new BaseService();
            _exportTargetTemplateService = exportTargetTemplateService ?? new ExportTargetTemplateService();
            _targetTemplateService = targetTemplateService ?? new TargetTemplateService();
        }

        public ActionResult ExportTargetTemplateKPIs(int targetTemplateId)
        {
            var criteria = new GetTargetTemplateByIdCriteria
            {
                TargetTemplateId = targetTemplateId
            };

            var kpisCriteria = new GetKPIOfTargetTemplateCriteria
            {
                TargetTemplateId = targetTemplateId
            };

            TargetTemplate targetTemplate = _baseService.GetDataEntity<TargetTemplate>(criteria);
            IEnumerable<TargetTemplateKPIGroupModel> kpis = _targetTemplateService.GetKPIOfTargetTemplate(kpisCriteria, false);

            _exportTargetTemplateService.Export(targetTemplate, kpis, Server.MapPath(TargetTemplateKPIsTemplate));

            return new EmptyResult();
        }
    }
}