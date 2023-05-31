namespace SMS.EPA.Site.Controllers.API.JobLevelStandard
{
    using System.Collections.Generic;
    using System.Web.Http;
    using HRM.UserControl.Client.Core.Enumerables;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevelStandard.Template;
    using SMS.EPA.Criteria.JobLevelStandard.TemplateSetting;
    using SMS.EPA.Model.JobLevelStandard.Template;
    using SMS.EPA.Model.JobLevelStandard.TemplateSetting;

    public class JobLevelStandardTemplateController : BaseController
    {
        private readonly IBaseService baseService;

        public JobLevelStandardTemplateController()
            : this(null)
        {
        }

        public JobLevelStandardTemplateController(IBaseService baseService)
        {
            this.baseService = baseService ?? new BaseService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardTemplate, Policy = Policy.Read)]
        public IEnumerable<JobLevelStandardTemplate> GetJobLevelStandardTemplate([FromUri] GetJobLevelStandardTemplateCriteria criteria)
        {
            return baseService.GetData<JobLevelStandardTemplate>(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardTemplate, Policy = Policy.Host)]
        public CommandResult CreateJobLevelStandardTemplate(CreateJobLevelStandardTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = CurrentEmployeeId;
            }

            return baseService.ExecuteNonQuery(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardTemplate, Policy = Policy.Host)]
        public CommandResult UpdateStatusJobLevelStandardTemplate(UpdateStatusJobLevelStandardTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentEmployeeId;
            }

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardTemplate, Policy = Policy.Host)]
        public CommandResult DeleteJobLevelStandardTemplate(int templateId)
        {
            var criteria = new DeleteJobLevelStandardTemplateCriteria()
            {
                TemplateId = templateId,
                ModifiedBy = CurrentEmployeeId
            };

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardTemplate, Policy = Policy.Read)]
        public JobLevelStandardTemplate GetJobLevelStandardTemplateById(int templateId)
        {
            var criteria = new GetJobLevelStandardTemplateByIdCriteria
            {
                TemplateId = templateId
            };

            return baseService.GetDataEntity<JobLevelStandardTemplate>(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardTemplate, Policy = Policy.Host)]
        public CommandResult UpdateJobLevelStandardTemplate(UpdateJobLevelStandardTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentEmployeeId;
            }

            return baseService.ExecuteNonQuery(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardTemplate, Policy = Policy.Read)]
        public IEnumerable<JobLevelStandardTemplateKPI> GetKPIListSelected([FromUri] GetKPIListSelectedCriteria criteria)
        {
            return baseService.GetData<JobLevelStandardTemplateKPI>(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardTemplate, Policy = Policy.Host)]
        public CommandResult UpdateTemplateKPI(UpdateJobLevelStandardTemplateKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentEmployeeId;
            }

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            return commandResult;
        }

        public byte GetUserPermissionOnKPI()
        {
            return GetPermissionOnFunction(EPAFunctionKey.JobLevelStandardTemplate);
        }
    }
}