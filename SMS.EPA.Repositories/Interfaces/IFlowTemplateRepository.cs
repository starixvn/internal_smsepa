namespace SMS.EPA.Repositories
{
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;

    public interface IFlowTemplateRepository
    {
        IEnumerable<FlowTemplateModel> GetFlowTemplate(ICriteria criteria);

        FlowTemplateModel GetFlowTemplateById(ICriteria criteria);
    }
}