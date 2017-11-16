namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using Criteria;
    using Interfaces;
    using Model;
    using Repositories;

    public class EvaluatorByKPIService : IEvaluatorByKPIService
    {
        private readonly IEmployeeEPARepository evaluationRepository;

        public EvaluatorByKPIService() : this(null)
        {
        }

        public EvaluatorByKPIService(IEmployeeEPARepository evaluationRepository)
        {
            this.evaluationRepository = evaluationRepository ?? new EmployeeEPARepository();
        }

        public IEnumerable<TemporaryKPIModel> GetKPIByTargetTemplate(GetKPIByTargetTemplateCriteria criteria)
        {
            IEnumerable<TemporaryKPIModel> kpis = new List<TemporaryKPIModel>();

            if (criteria != null && criteria.IsValid())
            {
                kpis = evaluationRepository.GetKPIByTargetTemplate(criteria);
            }

            return kpis;
        }

        public IEnumerable<TemporaryKPIModel> GetKPIToView(GetKPIToViewCriteria criteria)
        {
            IEnumerable<TemporaryKPIModel> kpis = new List<TemporaryKPIModel>();

            if (criteria != null && criteria.IsValid())
            {
                kpis = evaluationRepository.GetKPIToView(criteria);
            }

            return kpis;
        }

        public IEnumerable<RatingByKPIModel> GetEmployeeEPAToView(GetEmployeeEPAToViewCriteria criteria)
        {
            IEnumerable<RatingByKPIModel> epas = new List<RatingByKPIModel>();

            if (criteria != null && criteria.IsValid())
            {
                epas = evaluationRepository.GetEmployeeEPAByKPI(criteria);
            }

            return epas;
        }

        public IEnumerable<RatingByKPIModel> GetEmployeeEpabyKPI(GetEmployeeEPAByKPICriteria criteria)
        {
            IEnumerable<RatingByKPIModel> epas = new List<RatingByKPIModel>();

            if (criteria != null && criteria.IsValid())
            {
                epas = evaluationRepository.GetEmployeeEPAByKPI(criteria);
            }

            return epas;
        }
    }
}