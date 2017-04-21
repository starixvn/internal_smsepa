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

    public class ReviewerEPAService : BaseService, IReviewerEPAService
    {
        private readonly IEmployeeEPARepository _evaluationRepository;
        private readonly IEmployeeEPAService _evaluationService;

        public ReviewerEPAService()
            : this(null, null)
        {
        }

        public ReviewerEPAService(
            IEmployeeEPARepository evaluationRepository,
            IEmployeeEPAService evaluationService)
        {
            _evaluationRepository = evaluationRepository ?? new EmployeeEPARepository();
            _evaluationService = evaluationService ?? new EmployeeEPAService();
        }

        public IEnumerable<EPAModel> GetEPAByReviewer(GetEPAByReviewerCriteria criteria)
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
                        _evaluationService.CloneEPAProcess(epaInfo.Process.Reviewers.FirstOrDefault(x => x.EvaluatorInfo.EmployeeId == criteria.ReviewerId));
                    _evaluationService.SetMaxPerformanceScore(epaInfo);
                    _evaluationService.GroupMonthlyEPABySemi(epaInfo, monthlyEPAs);
                    _evaluationService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                    _evaluationService.SetMaxSubtractAndMaxAdditionScore(epaInfo);
                }
            }

            return epaInfos;
        }

        public EPAModel GetEPAInfoByReviewer(GetEPAInfoByReviewerCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = _evaluationRepository.GetEPAInfo(criteria);
                epaInfo.Process.CurrentProcess =
                    _evaluationService.CloneEPAProcess(epaInfo.Process.Reviewers.FirstOrDefault(x => x.EvaluatorInfo.EmployeeId == criteria.ReviewerId));
                _evaluationService.SetMaxPerformanceScore(epaInfo);
                _evaluationService.SpreadKPIScore(epaInfo.KPIGroups);

                if (epaInfo.IsSemiHasMonthly)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = _evaluationService.GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    _evaluationService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }

                if (epaInfo.IsProbation || epaInfo.IsPromotion || epaInfo.IsSemiAnnual)
                {
                    epaInfo.Recommendation = _evaluationService.GetRecommendationByEPAId(epaInfo.EPAId);
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