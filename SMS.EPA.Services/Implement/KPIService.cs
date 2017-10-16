namespace SMS.EPA.Services.Implement
{
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using Repositories;
    using System.Collections.Generic;

    public class KPIService : BaseService, IKPIService
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

        public CommandResult CreateKPI(CreateKPICriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateKPI(UpdateKPICriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateStatusKPI(UpdateStatusKPICriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateRequiredEvaluateKPI(UpdateRequiredEvaluateKPICriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteKPI(DeleteKPICriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }
    }
}