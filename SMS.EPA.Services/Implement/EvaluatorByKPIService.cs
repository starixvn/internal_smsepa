namespace SMS.EPA.Services.Implement
{
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using Model.Enitites;
    using Repositories;
    using System.Collections.Generic;

    public class EvaluatorByKPIService : BaseService, IEvaluatorByKPIService
    {
        private readonly IEmployeeEPARepository evaluationRepository;

        public EvaluatorByKPIService() : this(null)
        {
        }

        public EvaluatorByKPIService(IEmployeeEPARepository evaluationRepository)
        {
            this.evaluationRepository = evaluationRepository ?? new EmployeeEPARepository();
        }

        public IEnumerable<EPAPeriodModel> GetPeriodByEvaluator(GetPeriodByEvaluatorCriteria criteria)
        {
            IEnumerable<EPAPeriodModel> periods = new List<EPAPeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                periods = GetData<EPAPeriodModel>(criteria);
            }

            return periods;
        }

        public IEnumerable<TargetTemplate> GetTargetTemplateByEvaluator(GetTargetTemplateByEvaluatorCriteria criteria)
        {
            IEnumerable<TargetTemplate> targetTemplates = new List<TargetTemplate>();

            if (criteria != null && criteria.IsValid())
            {
                targetTemplates = GetData<TargetTemplate>(criteria);
            }

            return targetTemplates;
        }

        public IEnumerable<EPAPeriodModel> GetPeriodToView(GetPeriodToViewCriteria criteria)
        {
            IEnumerable<EPAPeriodModel> periods = new List<EPAPeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                periods = GetData<EPAPeriodModel>(criteria);
            }

            return periods;
        }

        public IEnumerable<TargetTemplate> GetTargetTemplateToView(GetTargetTemplateToViewCriteria criteria)
        {
            IEnumerable<TargetTemplate> targetTemplates = new List<TargetTemplate>();

            if (criteria != null && criteria.IsValid())
            {
                targetTemplates = GetData<TargetTemplate>(criteria);
            }

            return targetTemplates;
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

        public CommandResult UpdateKPIScore(UpdateKPIScoreCriteria criteria)
        {
            var commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public IEnumerable<Employee> GetEvaluatorsByTargetTemplate(GetEvaluatorsByTargetTemplateCriteria criteria)
        {
            IEnumerable<Employee> employees = new List<Employee>();

            if (criteria != null && criteria.IsValid())
            {
                employees = GetData<Employee>(criteria);
            }

            return employees;
        }
    }
}