namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Core.Services;
    using Fanex.Data.Repository;
    using Interfaces;
    using Model;
    using Repositories;

    public abstract class BaseActorEPAService : BaseService
    {
        protected readonly IEmployeeEPARepository EvaluationRepository;
        protected readonly IEmployeeEPAService EvaluationService;

        protected BaseActorEPAService()
            : this(null, null)
        {
        }

        protected BaseActorEPAService(
            IEmployeeEPARepository evaluationRepository,
            IEmployeeEPAService evaluationService)
        {
            EvaluationRepository = evaluationRepository ?? new EmployeeEPARepository();
            EvaluationService = evaluationService ?? new EmployeeEPAService();
        }

        public IEnumerable<EPAModel> GetEPAByActor(ICriteria criteria)
        {
            IEnumerable<EPAModel> epaInfos = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                epaInfos = EvaluationRepository.GetEPA(criteria);

                IList<EPAModel> monthlyEPAs = new List<EPAModel>();
                string epaIds = string.Join(",", epaInfos.Where(x => x.IsSemiHasMonthly).Select(x => x.EPAId));

                if (!string.IsNullOrEmpty(epaIds))
                {
                    monthlyEPAs = EvaluationService.GetMonthlyEPAOfSemi(epaIds);
                }

                UpdateEPAInfos(criteria, epaInfos, monthlyEPAs);
            }

            return epaInfos;
        }

        protected abstract void UpdateEPAInfos(
            ICriteria criteria,
            IEnumerable<EPAModel> epaInfos,
            IList<EPAModel> monthlyEPAs);
    }
}
