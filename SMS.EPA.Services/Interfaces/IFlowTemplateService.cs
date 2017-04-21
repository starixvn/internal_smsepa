namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Model;

    public interface IFlowTemplateService
    {
        IEnumerable<FlowTemplateModel> GetFlowTemplate(GetFlowTemplateCriteria criteria);

        FlowTemplateModel GetFlowTemplateById(int flowTemplateById);

        CommandResult CreateFlowTemplate(CreateFlowTemplateCriteria criteria);

        CommandResult UpdateFlowTemplate(UpdateFlowTemplateCriteria criteria);

        CommandResult UpdateStatusFlowTemplate(UpdateStatusFlowTemplateCriteria criteria);

        CommandResult DeleteFlowTemplate(DeleteFlowTemplateCriteria criteria);
    }
}