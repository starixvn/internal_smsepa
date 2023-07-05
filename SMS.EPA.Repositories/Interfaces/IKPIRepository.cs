namespace SMS.EPA.Repositories
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Model;

    public interface IKPIRepository
    {
        IEnumerable<KPIModel> GetKPI(ICriteria criteria);
    }
}