namespace SMS.EPA.Repositories
{
    using Fanex.Data.Repository;
    using Model;
    using System.Collections.Generic;

    public interface IKPIRepository
    {
        IEnumerable<KPIModel> GetKPI(ICriteria criteria);
    }
}