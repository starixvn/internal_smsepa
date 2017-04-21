namespace SMS.EPA.Services.Implement
{
    using Core.Model;
    using Core.Services;
    using Criteria;
    using Interfaces;
    using Model;
    using Repositories;
    using System.Collections.Generic;
    using System.Linq;

    public class EvaluatorEPAService : BaseService, IEvaluatorEPAService
    {
        private readonly IEmployeeEPARepository _evaluationRepository;
        private readonly IEmployeeEPAService _evaluationService;

        public EvaluatorEPAService()
            : this(null, null)
        {
        }

        public EvaluatorEPAService(
            IEmployeeEPARepository evaluationRepository,
            IEmployeeEPAService evaluationService)
        {
            _evaluationRepository = evaluationRepository ?? new EmployeeEPARepository();
            _evaluationService = evaluationService ?? new EmployeeEPAService();
        }

        public IEnumerable<EPAModel> GetEPAByEvaluator(GetEPAByEvaluatorCriteria criteria)
        {
            IEnumerable<EPAModel> epaInfos = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                epaInfos = _evaluationRepository.GetEPA(criteria);

                IList<EPAModel> monthlyEPAs = new List<EPAModel>();
                string epaIds = string.Join(",", epaInfos.Where(x => x.IsSemiHasMonthly).Select(x => x.EPAId));

                if (!string.IsNullOrEmpty(epaIds))
                {
                    monthlyEPAs = _evaluationService.GetMonthlyEPAOfSemi(epaIds);
                }

                foreach (EPAModel epaInfo in epaInfos)
                {
                    epaInfo.Process.CurrentProcess =
                        _evaluationService.CloneEPAProcess(
                            epaInfo.Process.Evaluators.FirstOrDefault(
                                x => x.EvaluatorInfo.EmployeeId == criteria.EvaluatorId));
                    _evaluationService.SetMaxPerformanceScore(epaInfo);
                    _evaluationService.GroupMonthlyEPABySemi(epaInfo, monthlyEPAs);
                    _evaluationService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }
            }

            return epaInfos;
        }

        public EPAModel GetEPAInfoByEvaluator(GetEPAInfoByEvaluatorCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = _evaluationRepository.GetEPAInfo(criteria);
                epaInfo.Process.CurrentProcess =
                    _evaluationService.CloneEPAProcess(epaInfo.Process.Evaluators.FirstOrDefault(x => x.EvaluatorInfo.EmployeeId == criteria.EvaluatorId));
                _evaluationService.SetMaxPerformanceScore(epaInfo);
                _evaluationService.SpreadKPIScore(epaInfo.KPIGroups);

                foreach (EPAKpiModel kpi in epaInfo.KPIGroups.SelectMany(x => x.KPIs))
                {
                    kpi.CurrentEvaluation = kpi.EvaluationByEvaluators.FirstOrDefault();
                }

                if (epaInfo.IsSemiHasMonthly)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = _evaluationService.GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    _evaluationService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }
            }

            return epaInfo;
        }

        public CommandResult UpdateEPAScoreByEvaluator(UpdateEPAScoreCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }
    }
}