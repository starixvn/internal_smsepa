namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Model;

    public interface IFlowTemplateRepository
    {
        IEnumerable<FlowTemplateModel> GetFlowTemplate(ICriteria criteria);

        FlowTemplateModel GetFlowTemplateById(ICriteria criteria);
    }
}