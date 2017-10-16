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
        private readonly IEmployeeEPARepository evaluationRepository;
        private readonly IEmployeeEPAService evaluationService;

        public ManagementEPAService() : this(null, null)
        {
        }

        public ManagementEPAService(
            IEmployeeEPARepository evaluationRepository,
            IEmployeeEPAService evaluationService)
        {
            this.evaluationRepository = evaluationRepository ?? new EmployeeEPARepository();
            this.evaluationService = evaluationService ?? new EmployeeEPAService();
        }

        public IEnumerable<EPAModel> GetEPAByManagement(GetEPAByManagementCriteria criteria)
        {
            IEnumerable<EPAModel> memberEPAs = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                memberEPAs = evaluationRepository.GetEPA(criteria);

                IList<EPAModel> monthlyEPAs = new List<EPAModel>();
                string epaIds = string.Join(",", memberEPAs.Where(x => x.IsSemiHasMonthly).Select(x => x.EPAId));

                if (!string.IsNullOrEmpty(epaIds))
                {
                    monthlyEPAs = evaluationService.GetMonthlyEPAOfSemi(epaIds);
                }

                foreach (EPAModel memberEPA in memberEPAs)
                {
                    memberEPA.Process.CurrentProcess =
                        evaluationService.CloneEPAProcess(memberEPA.Process.Managements.FirstOrDefault(x => x.EvaluatorInfo.EmployeeId == criteria.ManagementId));
                    evaluationService.SetMaxPerformanceScore(memberEPA);
                    evaluationService.GroupMonthlyEPABySemi(memberEPA, monthlyEPAs);
                    evaluationService.ConvertAverageMonthlyScore(memberEPA.MonthlyEPA, memberEPA.ScoreRatingType);
                    evaluationService.SetMaxSubtractAndMaxAdditionScore(memberEPA);
                }
            }

            return memberEPAs;
        }

        public EPAModel GetEPAInfoByManagement(GetEPAInfoByManagementCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = evaluationRepository.GetEPAInfo(criteria);
                epaInfo.Process.CurrentProcess =
                    evaluationService.CloneEPAProcess(epaInfo.Process.Managements.FirstOrDefault(x => x.EvaluatorInfo.EmployeeId == criteria.ManagementId));
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

        public CommandResult AdjustScoreByManagement(AdjustScoreByReviewerOrManagementCriteria criteria)
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