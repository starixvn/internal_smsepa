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
        private readonly IFlowTemplateRepository flowTemplateRepository;

        public FlowTemplateService() : this(null)
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

        public FlowTemplateModel GetFlowTemplateById(int flowTemplateById)
        {
            var flowTemplate = new FlowTemplateModel();
            var criteria = new GetFlowTemplateByIdCriteria
            {
                FlowTemplateId = flowTemplateById
            };

            if (criteria.IsValid())
            {
                flowTemplate = flowTemplateRepository.GetFlowTemplateById(criteria);
            }

            return flowTemplate;
        }

        public CommandResult CreateFlowTemplate(CreateFlowTemplateCriteria criteria)
        {
            var result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.ExecuteNonQuery(criteria);
            }

            return result;
        }

        public CommandResult UpdateFlowTemplate(UpdateFlowTemplateCriteria criteria)
        {
            var result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.ExecuteNonQuery(criteria);
            }

            return result;
        }

        public CommandResult UpdateStatusFlowTemplate(UpdateStatusFlowTemplateCriteria criteria)
        {
            var result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.ExecuteNonQuery(criteria);
            }

            return result;
        }

        public CommandResult DeleteFlowTemplate(DeleteFlowTemplateCriteria criteria)
        {
            var result = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                result = Repository.ExecuteNonQuery(criteria);
            }

            return result;
        }
    }
}