namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Repositories;
    using SMS.EPA.Services.Interfaces;

    public class ReviewerEPAService : BaseService, IReviewerEPAService
    {
        private readonly IEmployeeEPARepository evaluationRepository;
        private readonly IEmployeeEPAService evaluationService;

        public ReviewerEPAService()
            : this(null, null)
        {
        }

        public ReviewerEPAService(
            IEmployeeEPARepository evaluationRepository,
            IEmployeeEPAService evaluationService)
        {
            this.evaluationRepository = evaluationRepository ?? new EmployeeEPARepository();
            this.evaluationService = evaluationService ?? new EmployeeEPAService();
        }

        public IEnumerable<EPAModel> GetEPAByReviewer(GetEPAByReviewerCriteria criteria)
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
                        evaluationService.CloneEPAProcess(epaInfo.Process.Reviewers.FirstOrDefault(x => x.EvaluatorInfo.EmployeeId == criteria.ReviewerId));
                    evaluationService.SetMaxPerformanceScore(epaInfo);
                    evaluationService.GroupMonthlyEPABySemi(epaInfo, monthlyEPAs);
                    evaluationService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                    evaluationService.SetMaxSubtractAndMaxAdditionScore(epaInfo);
                }
            }

            return epaInfos;
        }

        public EPAModel GetEPAInfoByReviewer(GetEPAInfoByReviewerCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = evaluationRepository.GetEPAInfo(criteria);
                epaInfo.Process.CurrentProcess =
                    evaluationService.CloneEPAProcess(epaInfo.Process.Reviewers.FirstOrDefault(x => x.EvaluatorInfo.EmployeeId == criteria.ReviewerId));
                evaluationService.SetMaxPerformanceScore(epaInfo);
                evaluationService.SpreadKPIScore(epaInfo.KPIGroups);

                if (epaInfo.IsSemiHasMonthly)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = evaluationService.GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    evaluationService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }

                if (epaInfo.IsProbation || epaInfo.IsPromotion || epaInfo.IsSemiAnnual)
                {
                    epaInfo.Recommendation = evaluationService.GetRecommendationByEPAId(epaInfo.EPAId);
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