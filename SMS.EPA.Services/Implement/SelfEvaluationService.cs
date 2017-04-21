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

    public class SelfEvaluationService : BaseService, ISelfEvaluationService
    {
        private readonly IEmployeeEPARepository _employeeEPARepository;
        private readonly IEmployeeEPAService _evaluationService;

        public SelfEvaluationService()
            : this(null, null)
        {
        }

        public SelfEvaluationService(
            IEmployeeEPARepository employeeEPARepository,
            IEmployeeEPAService evaluationService)
        {
            _employeeEPARepository = employeeEPARepository ?? new EmployeeEPARepository();
            _evaluationService = evaluationService ?? new EmployeeEPAService();
        }

        public IEnumerable<EPAModel> GetEPAByEmployee(GetEPAByEmployeeCriteria criteria)
        {
            IEnumerable<EPAModel> epas = new List<EPAModel>();

            if (criteria != null && criteria.IsValid())
            {
                epas = _employeeEPARepository.GetEPA(criteria);

                foreach (EPAModel epa in epas)
                {
                    epa.Process.CurrentProcess = _evaluationService.CloneEPAProcess(epa.Process.Self);
                }
            }

            return epas;
        }

        public EPAModel GetEPAInfoByEmployee(GetEPAInfoByEmployeeCriteria criteria)
        {
            EPAModel epaInfo = new EPAModel();

            if (criteria != null && criteria.IsValid())
            {
                epaInfo = _employeeEPARepository.GetEPAInfo(criteria);
                epaInfo.Process.CurrentProcess = _evaluationService.CloneEPAProcess(epaInfo.Process.Self);
                _evaluationService.SetMaxPerformanceScore(epaInfo);
                _evaluationService.SpreadKPIScore(epaInfo.KPIGroups);

                foreach (EPAKpiModel kpi in epaInfo.KPIGroups.SelectMany(x => x.KPIs))
                {
                    kpi.CurrentEvaluation = kpi.EvaluationBySelf;
                }

                if (epaInfo.IsSemiHasMonthly)
                {
                    epaInfo.MonthlyEPA.MonthlyEPAs = _evaluationService.GetMonthlyEPAOfSemi(epaInfo.EPAId.ToString());
                    _evaluationService.ConvertAverageMonthlyScore(epaInfo.MonthlyEPA, epaInfo.ScoreRatingType);
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