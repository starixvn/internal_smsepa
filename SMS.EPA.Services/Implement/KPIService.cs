namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.KPI;
    using SMS.EPA.Model.KPI;
    using SMS.EPA.Repositories.Implement;
    using SMS.EPA.Repositories.Interfaces;
    using SMS.EPA.Services.Interfaces;

    public class KPIService : BaseService, IKPIService
    {
        private readonly IKPIRepository _kpiRepository;

        public KPIService()
            : this(null)
        {
        }

        public KPIService(IKPIRepository kpiRepository)
        {
            _kpiRepository = kpiRepository ?? new KPIRepository();
        }

        public IEnumerable<KPIModel> GetKPI(GetKPICriteria criteria)
        {
            IEnumerable<KPIModel> kpis = new List<KPIModel>();

            if (criteria != null && criteria.IsValid())
            {
                kpis = _kpiRepository.GetKPI(criteria);
            }

            return kpis;
        }

        public CommandResult CreateKPI(CreateKPICriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateKPI(UpdateKPICriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateStatusKPI(UpdateStatusKPICriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateRequiredEvaluateKPI(UpdateRequiredEvaluateKPICriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult DeleteKPI(DeleteKPICriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Delete(criteria);
            }

            return commandResult;
        }
    }
}