namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using Criteria;
    using Interfaces;
    using Model;
    using Repositories;

    public class KPIService : IKPIService
    {
        private readonly IKPIRepository kpiRepository;

        public KPIService() : this(null)
        {
        }

        public KPIService(IKPIRepository kpiRepository)
        {
            this.kpiRepository = kpiRepository ?? new KPIRepository();
        }

        public IEnumerable<KPIModel> GetKPI(GetKPICriteria criteria)
        {
            IEnumerable<KPIModel> kpis = new List<KPIModel>();

            if (criteria != null && criteria.IsValid())
            {
                kpis = kpiRepository.GetKPI(criteria);
            }

            return kpis;
        }
    }
}