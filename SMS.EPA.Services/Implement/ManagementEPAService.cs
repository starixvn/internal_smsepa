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

    public class ManagementEPAService : BaseService, IManagementEPAService
    {
        private readonly IEmployeeEPARepository _evaluationRepository;
        private readonly IEmployeeEPAService _evaluationService;

        public ManagementEPAService()
            : this(null, null)
        {
        }

        public ManagementEPAService(
            IEmployeeEPARepository evaluationRepository,
            IEmployeeEPAService evaluationService)
        {
            _evaluationRepository = evaluationRepository ?? new EmployeeEPARepository();
            _evaluationService = evaluationService ?? new EmployeeEPAService();
        }

        public IEnumerable<EPAModel> GetEPAByManagement(GetEPAByManagementCriteria criteria)
        {
            IEnumerable<EPAModel> memberEPAs = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                memberEPAs = _evaluationRepository.GetEPA(criteria);

                IList<EPAModel> monthlyEPAs = new List<EPAModel>();
                string epaIds = string.Join(",", memberEPAs.Where(x => x.IsSemiHasMonthly).Select(x => x.EPAId));

                if (!string.IsNullOrEmpty(epaIds))
                {
                    monthlyEPAs = _evaluationService.GetMonthlyEPAOfSemi(epaIds);
                }

                foreach (EPAModel memberEPA in memberEPAs)
                {
                    memberEPA.Process.CurrentProcess =
                        _evaluationService.CloneEPAProcess(memberEPA.Process.Managements.FirstOrDefault(x => x.EvaluatorInfo.EmployeeId == criteria.ManagementId));
                    _evaluationService.SetMaxPerformanceScore(memberEPA);
                    _evaluationService.GroupMonthlyEPABySemi(memberEPA, monthlyEPAs);
                    _evaluationService.ConvertAverageMonthlyScore(memberEPA.MonthlyEPA, memberEPA.ScoreRatingType);
                    _evaluationService.SetMaxSubtractAndMaxAdditionScore(memberEPA);
                }
            }

            return memberEPAs;
        }

        public EPAModel GetEPAInfoByManagement(GetEPAInfoByManagementCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = _evaluationRepository.GetEPAInfo(criteria);
                epaInfo.Process.CurrentProcess =
                    _evaluationService.CloneEPAProcess(epaInfo.Process.Managements.FirstOrDefault(x => x.EvaluatorInfo.EmployeeId == criteria.ManagementId));
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

        public CommandResult AdjustScoreByManagement(AdjustScoreByReviewerOrManagementCriteria criteria)
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