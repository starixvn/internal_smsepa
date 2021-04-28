namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Criteria;
    using Fanex.Data.Repository;
    using Interfaces;
    using Model;

    public class ReviewerEPAService : BaseActorEPAService, IReviewerEPAService
    {
        private readonly ICommonService commonService;

        public ReviewerEPAService() : this(null)
        {
        }

        public ReviewerEPAService(ICommonService commonService)
            : base(null, null)
        {
            this.commonService = commonService ?? new CommonService();
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
                        EvaluationService.CloneEPAProcess(epaInfo.Process.Reviewers.FirstOrDefault(x => x.EvaluatorInfo.EmployeeId == getEPAByReviewerCriteria.ReviewerId));
                }

                EvaluationService.SetMaxPerformanceScore(epaInfo);
                EvaluationService.GroupMonthlyEPABySemi(epaInfo, monthlyEPAs);
                EvaluationService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                EvaluationService.SetMaxSubtractAndMaxAdditionScore(epaInfo);
            }
        }

        public EPAModel GetEPAInfoByReviewer(GetEPAInfoByReviewerCriteria criteria)
        {
            var epaInfo = new EPAModel();
            if (criteria != null && criteria.IsValid())
            {
                epaInfo = EvaluationRepository.GetEPAInfo(criteria);
                epaInfo.JobDescriptions = commonService.GetJobDescriptionByEPAId(criteria.EPAId);
                epaInfo.Process.CurrentProcess =
                    EvaluationService.CloneEPAProcess(epaInfo.Process.Reviewers.FirstOrDefault(x => x.EvaluatorInfo.EmployeeId == criteria.ReviewerId));
                EvaluationService.SetMaxPerformanceScore(epaInfo);
                EvaluationService.SpreadKPIScore(epaInfo.KPIGroups);

                if (epaInfo.IsSemiHasMonthly)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = EvaluationService.GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    EvaluationService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }

                if (epaInfo.IsProbation || epaInfo.IsPromotion || epaInfo.IsSemiAnnual)
                {
                    epaInfo.Recommendations = EvaluationService.GetRecommendationByEPAId(epaInfo.EPAId);
                }

                if (epaInfo.IsProbation)
                {
                    epaInfo.ExtendedQuestion = EvaluationService.GetExtendedQuestionByEPAId(epaInfo.EPAId);
                }
            }

            return epaInfo;
        }
    }
}