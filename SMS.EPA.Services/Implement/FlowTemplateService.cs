namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class FlowTemplateService : BaseService, IFlowTemplateService
    {
        private readonly IFlowTemplateRepository flowTemplateRepository;

        public FlowTemplateService()
            : this(null)
        {
        }

        public FlowTemplateService(IFlowTemplateRepository flowTemplateRepository)
        {
            this.flowTemplateRepository = flowTemplateRepository ?? new FlowTemplateRepository();
        }

        public IEnumerable<FlowTemplateModel> GetFlowTemplate(GetFlowTemplateCriteria criteria)
        {
            IEnumerable<FlowTemplateModel> flowTemplates = new List<FlowTemplateModel>();

            if (criteria != null && criteria.IsValid())
            {
                flowTemplates = flowTemplateRepository.GetFlowTemplate(criteria);
            }

            return flowTemplates;
        }

        public FlowTemplateModel GetFlowTemplateById(GetFlowTemplateByIdCriteria criteria)
        {
            FlowTemplateModel flowTemplate = new FlowTemplateModel();

            if (criteria != null && criteria.IsValid())
            {
                flowTemplate = flowTemplateRepository.GetFlowTemplateById(criteria);
            }

            return flowTemplate;
        }

        public CommandResult CreateFlowTemplate(CreateFlowTemplateCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Insert(criteria);
            }

            return result;
        }

        public CommandResult UpdateFlowTemplate(UpdateFlowTemplateCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        public CommandResult UpdateStatusFlowTemplate(UpdateStatusFlowTemplateCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Update(criteria);
            }

            return result;
        }

        public CommandResult DeleteFlowTemplate(DeleteFlowTemplateCriteria criteria)
        {
            CommandResult result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.Delete(criteria);
            }

            return result;
        }
    }
}