namespace SMS.EPA.Repositories
{
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;

    public interface ITargetTemplateRepository
    {
        IEnumerable<KPIModel> GetKPIOfTargetTemplate(ICriteria criteria);
    }
}