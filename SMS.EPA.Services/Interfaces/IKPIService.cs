namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Criteria;
    using Model;

    public interface IKPIService
    {
        IEnumerable<KPIModel> GetKPI(GetKPICriteria criteria);
    }
}