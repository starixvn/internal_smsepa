namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Enitites;
    using SMS.EPA.Repositories;
    using SMS.EPA.Services.Interfaces;

    public class EvaluatorByKPIService : BaseService, IEvaluatorByKPIService
    {
        private readonly IEmployeeEPARepository evaluationRepository;

        public EvaluatorByKPIService()
            : this(null)
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
                epas = evaluationRepository.GetEmployeeEPAToView(criteria);
            }

            return epas;
        }

        public IEnumerable<RatingByKPIModel> GetEmployeeEPAByKPI(GetEmployeeEPAByKPICriteria criteria)
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
            CommandResult commandResult = new CommandResult();

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