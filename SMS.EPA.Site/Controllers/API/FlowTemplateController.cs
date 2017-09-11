namespace SMS.EPA.Site.Controllers.API
{
    using Core.Attribute;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Web.Http;

    public class FlowTemplateController : BaseController
    {
        private readonly IFlowTemplateService flowTemplateService;

        public FlowTemplateController() : this(null)
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
            var criteria = new GetFlowTemplateCriteria
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
            return flowTemplateService.GetFlowTemplateById(flowTemplateId);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult CreateFlowTemplate(CreateFlowTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = SessionService.EmployeeId;
            }

            return flowTemplateService.CreateFlowTemplate(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult UpdateFlowTemplate(UpdateFlowTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            return flowTemplateService.UpdateFlowTemplate(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult UpdateStatusFlowTemplate(UpdateStatusFlowTemplateCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            return flowTemplateService.UpdateStatusFlowTemplate(criteria);
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult DeleteFlowTemplate(int flowTemplateId)
        {
            var criteria = new DeleteFlowTemplateCriteria
            {
                FlowTemplateId = flowTemplateId,
                ModifiedBy = SessionService.EmployeeId
            };

            return flowTemplateService.DeleteFlowTemplate(criteria);
        }
    }
}