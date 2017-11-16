namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using Criteria;
    using Interfaces;
    using Model;
    using Repositories;

    public class FlowTemplateService : IFlowTemplateService
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
    }
}