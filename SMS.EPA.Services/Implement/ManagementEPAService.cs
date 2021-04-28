namespace SMS.EPA.Services.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using Criteria;
    using Interfaces;
    using Model;
    using Repositories;

    public class ManagementEPAService : IManagementEPAService
    {
        private readonly IEmployeeEPARepository evaluationRepository;
        private readonly IEmployeeEPAService evaluationService;
        private readonly ICommonService commonService;

        public ManagementEPAService() : this(null, null, null)
        {
        }

        public ManagementEPAService(
            IEmployeeEPARepository evaluationRepository,
            IEmployeeEPAService evaluationService,
            ICommonService commonService)
        {
            this.evaluationRepository = evaluationRepository ?? new EmployeeEPARepository();
            this.evaluationService = evaluationService ?? new EmployeeEPAService();
            this.commonService = commonService ?? new CommonService();
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
            var epaInfo = new EPAModel();
            if (criteria != null && criteria.IsValid())
            {
                epaInfo = evaluationRepository.GetEPAInfo(criteria);
                epaInfo.JobDescriptions = commonService.GetJobDescriptionByEPAId(criteria.EPAId);
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
                    epaInfo.Recommendations = evaluationService.GetRecommendationByEPAId(epaInfo.EPAId);
                }

                if (epaInfo.IsProbation)
                {
                    epaInfo.ExtendedQuestion = evaluationService.GetExtendedQuestionByEPAId(epaInfo.EPAId);
                }
            }

            return epaInfo;
        }
    }
}