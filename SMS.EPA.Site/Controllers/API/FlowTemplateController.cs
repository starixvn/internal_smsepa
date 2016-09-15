namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class FlowTemplateController : BaseController
    {
        private readonly IFlowTemplateService flowTemplateService;

        public FlowTemplateController()
            : this(null)
        {
        }

        public FlowTemplateController(IFlowTemplateService flowTemplateService)
        {
            this.flowTemplateService = flowTemplateService ?? new FlowTemplateService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<FlowTemplateModel> GetFlowTemplate(
            int? targetTypeId = null,
            bool? isActive = null)
        {
            var criteria = new GetFlowTemplateCriteria()
            {
                TargetTypeId = targetTypeId,
                IsActive = isActive
            };

            return flowTemplateService.GetFlowTemplate(criteria);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public FlowTemplateModel GetFlowTemplateById(int flowTemplateId)
        {
            var criteria = new GetFlowTemplateByIdCriteria()
            {
                FlowTemplateId = flowTemplateId
            };

            return flowTemplateService.GetFlowTemplateById(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult CreateFlowTemplate(CreateFlowTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = this.SessionService.EmployeeId;
            }

            return flowTemplateService.CreateFlowTemplate(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult UpdateFlowTemplate(UpdateFlowTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = this.SessionService.EmployeeId;
            }

            return flowTemplateService.UpdateFlowTemplate(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult UpdateStatusFlowTemplate(UpdateStatusFlowTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = this.SessionService.EmployeeId;
            }

            return flowTemplateService.UpdateStatusFlowTemplate(criteria);
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult DeleteFlowTemplate(int flowTemplateId)
        {
            var criteria = new DeleteFlowTemplateCriteria()
            {
                FlowTemplateId = flowTemplateId,
                ModifiedBy = this.SessionService.EmployeeId
            };

            return flowTemplateService.DeleteFlowTemplate(criteria);
        }
    }
}