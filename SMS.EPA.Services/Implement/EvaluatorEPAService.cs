namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories;
    using SMS.EPA.Services.Interfaces;

    public class EvaluatorEPAService : BaseService, IEvaluatorEPAService
    {
        private readonly IEmployeeEPARepository evaluationRepository;
        private readonly IEmployeeEPAService evaluationService;

        public EvaluatorEPAService()
            : this(null, null)
        {
        }

        public EvaluatorEPAService(
            IEmployeeEPARepository evaluationRepository,
            IEmployeeEPAService evaluationService)
        {
            this.evaluationRepository = evaluationRepository ?? new EmployeeEPARepository();
            this.evaluationService = evaluationService ?? new EmployeeEPAService();
        }

        public IEnumerable<EPAModel> GetEPAByEvaluator(GetEPAByEvaluatorCriteria criteria)
        {
            IEnumerable<EPAModel> epaInfos = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                epaInfos = evaluationRepository.GetEPA(criteria);

                IList<EPAModel> monthlyEPAs = new List<EPAModel>();
                string epaIds = string.Join(",", epaInfos.Where(x => x.IsSemiHasMonthly).Select(x => x.EPAId));

                if (!string.IsNullOrEmpty(epaIds))
                {
                    monthlyEPAs = evaluationService.GetMonthlyEPAOfSemi(epaIds);
                }

                foreach (EPAModel epaInfo in epaInfos)
                {
                    epaInfo.Process.CurrentProcess =
                        evaluationService.CloneEPAProcess(epaInfo.Process.Evaluators.FirstOrDefault(x => x.EvaluatorInfo.EmployeeId == criteria.EvaluatorId));
                    evaluationService.SetMaxPerformanceScore(epaInfo);
                    evaluationService.GroupMonthlyEPABySemi(epaInfo, monthlyEPAs);
                    evaluationService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }
            }

            return epaInfos;
        }

        public EPAModel GetEPAInfoByEvaluator(GetEPAInfoByEvaluatorCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = evaluationRepository.GetEPAInfo(criteria);
                epaInfo.Process.CurrentProcess =
                    evaluationService.CloneEPAProcess(epaInfo.Process.Evaluators.FirstOrDefault(x => x.EvaluatorInfo.EmployeeId == criteria.EvaluatorId));
                evaluationService.SetMaxPerformanceScore(epaInfo);
                evaluationService.SpreadKPIScore(epaInfo.KPIGroups);

                foreach (EPAKpiModel kpi in epaInfo.KPIGroups.SelectMany(x => x.KPIs))
                {
                    kpi.CurrentEvaluation = kpi.EvaluationByEvaluators.FirstOrDefault();
                }

                if (epaInfo.IsSemiHasMonthly)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = evaluationService.GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    evaluationService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
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