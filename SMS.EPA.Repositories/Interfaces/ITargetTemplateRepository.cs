namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Model;

    public interface ITargetTemplateRepository
    {
        IEnumerable<KPIModel> GetKPIOfTargetTemplate(ICriteria criteria);
    }
}