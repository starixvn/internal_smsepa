namespace SMS.EPA.Services.Implement
{
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using Repositories;
    using System.Collections.Generic;

    public class FlowTemplateService : BaseService, IFlowTemplateService
    {
        private readonly IFlowTemplateRepository _flowTemplateRepository;

        public FlowTemplateService()
            : this(null)
        {
        }

        public FlowTemplateService(IFlowTemplateRepository flowTemplateRepository)
        {
            _flowTemplateRepository = flowTemplateRepository ?? new FlowTemplateRepository();
        }

        public IEnumerable<FlowTemplateModel> GetFlowTemplate(GetFlowTemplateCriteria criteria)
        {
            IEnumerable<FlowTemplateModel> flowTemplates = new List<FlowTemplateModel>();

            if (criteria != null && criteria.IsValid())
            {
                flowTemplates = _flowTemplateRepository.GetFlowTemplate(criteria);
            }

            return flowTemplates;
        }

        public FlowTemplateModel GetFlowTemplateById(int flowTemplateById)
        {
            FlowTemplateModel flowTemplate = new FlowTemplateModel();
            var criteria = new GetFlowTemplateByIdCriteria
            {
                FlowTemplateId = flowTemplateById
            };

            if (criteria.IsValid())
            {
                flowTemplate = _flowTemplateRepository.GetFlowTemplateById(criteria);
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