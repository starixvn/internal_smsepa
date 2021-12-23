namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories;

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