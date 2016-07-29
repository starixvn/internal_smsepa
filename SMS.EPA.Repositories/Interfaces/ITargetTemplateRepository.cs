namespace SMS.EPA.Repositories.Interfaces
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using SMS.EPA.Model;

    public interface ITargetTemplateRepository
    {
        IEnumerable<KPIModel> GetKPIOfTargetTemplate(ICriteria criteria);
    }
}