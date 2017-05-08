namespace SMS.EPA.Services.Implement
{
    using Core.Model;
    using Criteria;
    using Interfaces;
    using Model;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Data.Repository;

    public class ReviewerEPAService : BaseActorEPAService, IReviewerEPAService
    {
        public ReviewerEPAService()
            : base(null, null)
        {
        }

        public IEnumerable<EPAModel> GetEPAByReviewer(GetEPAByReviewerCriteria criteria)
        {
            return GetEPAByActor(criteria);
        }

        protected override void UpdateEPAInfos(
            ICriteria criteria,
            IEnumerable<EPAModel> epaInfos,
            IList<EPAModel> monthlyEPAs)
        {
            GetEPAByReviewerCriteria getEPAByReviewerCriteria = criteria as GetEPAByReviewerCriteria;

            foreach (EPAModel epaInfo in epaInfos)
            {
                if (getEPAByReviewerCriteria != null)
                {
                    epaInfo.Process.CurrentProcess =
                        EvaluationService.CloneEPAProcess(
                            epaInfo.Process.Reviewers.FirstOrDefault(
                                x => x.EvaluatorInfo.EmployeeId == getEPAByReviewerCriteria.ReviewerId));
                }

                EvaluationService.SetMaxPerformanceScore(epaInfo);
                EvaluationService.GroupMonthlyEPABySemi(epaInfo, monthlyEPAs);
                EvaluationService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                EvaluationService.SetMaxSubtractAndMaxAdditionScore(epaInfo);
            }
        }

        public EPAModel GetEPAInfoByReviewer(GetEPAInfoByReviewerCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = EvaluationRepository.GetEPAInfo(criteria);
                epaInfo.Process.CurrentProcess =
                    EvaluationService.CloneEPAProcess(
                        epaInfo.Process.Reviewers.FirstOrDefault(
                            x => x.EvaluatorInfo.EmployeeId == criteria.ReviewerId));
                EvaluationService.SetMaxPerformanceScore(epaInfo);
                EvaluationService.SpreadKPIScore(epaInfo.KPIGroups);

                if (epaInfo.IsSemiHasMonthly)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = EvaluationService.GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    EvaluationService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }

                if (epaInfo.IsProbation || epaInfo.IsPromotion || epaInfo.IsSemiAnnual)
                {
                    epaInfo.Recommendation = EvaluationService.GetRecommendationByEPAId(epaInfo.EPAId);
                }
            }

            return epaInfo;
        }

        public CommandResult AdjustScoreByReviewer(AdjustScoreByReviewerOrManagementCriteria criteria)
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