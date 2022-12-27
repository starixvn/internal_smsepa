namespace SMS.EPA.AutoService.Service
{
    using System.Collections.Generic;
    using System.Linq;
    using SMS.EPA.AutoService.Criteria;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Extensions;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Model;
    using SMS.EPA.Services;

    public class ProcessService : IProcessService
    {
        private readonly ICryptographyService cryptographyService;
        private readonly IBaseService baseService;
        private readonly IEPAProcessService processService;

        public ProcessService() : this(null, null, null)
        {
        }

        public ProcessService(
            ICryptographyService cryptographyService,
            IBaseService baseService,
            IEPAProcessService processService)
        {
            this.cryptographyService = cryptographyService ?? new CryptographyService();
            this.baseService = baseService ?? new BaseService();
            this.processService = processService ?? new EPAProcessService();
        }

        public void UpdateInfomationOfProcess()
        {
            var criteria = new CheckEndProcessCriteria();
            var processes = baseService.GetData<AutoEvaluationProcessModel>(criteria).ToList();
            DecryptTotalScore(processes);
            if (processes != null && processes.Any())
            {
                UpdateEndProcess(processes);
                processService.UpdateTotalScoreOfSemiHasMonthly(processes.Where(x => x.ProcessId == (byte)ProcessType.Evaluator && x.IsSemiHasMonthly));
                processService.UpdateTotalScoreOfProcess(processes.Where(x => x.ProcessId >= (byte)ProcessType.Reviewer));
            }
        }

        private void UpdateEndProcess(IEnumerable<AutoEvaluationProcessModel> processes)
        {
            var criteria = new UpdateEndProcessCriteria
            {
                EPAProcess = processes.ToXML().ToString()
            };
            baseService.ExecuteNonQuery(criteria);
        }

        private void DecryptTotalScore(IEnumerable<AutoEvaluationProcessModel> evaluationProcesses)
        {
            foreach (AutoEvaluationProcessModel evaluationProcess in evaluationProcesses)
            {
                evaluationProcess.TotalScore = cryptographyService.Decrypt(evaluationProcess.TotalScore);
            }
        }
    }
}