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

    public class SelfEvaluationService : BaseService, ISelfEvaluationService
    {
        private readonly IEmployeeEPARepository employeeEPARepository;
        private readonly IEmployeeEPAService evaluationService;

        public SelfEvaluationService()
            : this(null, null)
        {
        }

        public SelfEvaluationService(
            IEmployeeEPARepository employeeEPARepository,
            IEmployeeEPAService evaluationService)
        {
            this.employeeEPARepository = employeeEPARepository ?? new EmployeeEPARepository();
            this.evaluationService = evaluationService ?? new EmployeeEPAService();
        }

        public IEnumerable<EPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria)
        {
            IEnumerable<EPAModel> epas = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                epas = employeeEPARepository.GetEPA(criteria);

                foreach (EPAModel epa in epas)
                {
                    epa.Process.CurrentProcess = evaluationService.CloneEPAProcess(epa.Process.Self);
                }
            }

            return epas;
        }

        public EPAModel GetEPAInfoByEmployee(GetEPAInfoByEmployeeCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = employeeEPARepository.GetEPAInfo(criteria);
                epaInfo.Process.CurrentProcess = evaluationService.CloneEPAProcess(epaInfo.Process.Self);
                evaluationService.SetMaxPerformanceScore(epaInfo);
                evaluationService.SpreadKPIScore(epaInfo.KPIGroups);

                foreach (EPAKPIModel kpi in epaInfo.KPIGroups.SelectMany(x => x.KPIs))
                {
                    kpi.CurrentEvaluation = kpi.EvaluationBySelf;
                }

                if (epaInfo.IsSemiHasMonthly)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = evaluationService.GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    evaluationService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
                }
            }

            return epaInfo;
        }

        public CommandResult UpdateEPAScoreByEmployee(UpdateEPAScoreCriteria criteria)
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