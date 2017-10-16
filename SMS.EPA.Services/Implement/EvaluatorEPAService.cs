namespace SMS.EPA.Services.Implement
{
    using Core.Model;
    using Criteria;
    using Interfaces;
    using Model;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data.Repository;

    public class EvaluatorEPAService : BaseActorEPAService, IEvaluatorEPAService
    {
        public EvaluatorEPAService()
            : base(null, null)
        {
        }

        public IEnumerable<EPAModel> GetEPAByEvaluator(GetEPAByEvaluatorCriteria criteria)
        {
            return GetEPAByActor(criteria);
        }

        protected override void UpdateEPAInfos(
            ICriteria criteria,
            IEnumerable<EPAModel> epaInfos,
            IList<EPAModel> monthlyEPAs)
        {
            GetEPAByEvaluatorCriteria getEPAByEvaluatorCriteria = criteria as GetEPAByEvaluatorCriteria;

            foreach (EPAModel epaInfo in epaInfos)
            {
                if (getEPAByEvaluatorCriteria != null)
                {
                    epaInfo.Process.CurrentProcess =
                        EvaluationService.CloneEPAProcess(
                            epaInfo.Process.Evaluators.FirstOrDefault(
                                x => x.EvaluatorInfo.EmployeeId == getEPAByEvaluatorCriteria.EvaluatorId));
                }

                EvaluationService.SetMaxPerformanceScore(epaInfo);
                EvaluationService.GroupMonthlyEPABySemi(epaInfo, monthlyEPAs);
                EvaluationService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
            }
        }

        public EPAModel GetEPAInfoByEvaluator(GetEPAInfoByEvaluatorCriteria criteria)
        {
            var epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = EvaluationRepository.GetEPAInfo(criteria);
                epaInfo.Process.CurrentProcess =
                    EvaluationService.CloneEPAProcess(
                        epaInfo.Process.Evaluators.FirstOrDefault(
                            x => x.EvaluatorInfo.EmployeeId == criteria.EvaluatorId));
                EvaluationService.SetMaxPerformanceScore(epaInfo);
                EvaluationService.SpreadKPIScore(epaInfo.KPIGroups);

                foreach (EPAKpiModel kpi in epaInfo.KPIGroups.SelectMany(x => x.KPIs))
                {
                    kpi.CurrentEvaluation = kpi.EvaluationByEvaluators.FirstOrDefault();
                }

                if (epaInfo.IsSemiHasMonthly)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = EvaluationService.GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    EvaluationService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }
            }

            return epaInfo;
        }

        public CommandResult UpdateEPAScoreByEvaluator(UpdateEPAScoreCriteria criteria)
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