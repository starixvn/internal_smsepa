namespace SMS.EPA.AutoService.Service
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using log4net;
    using SMS.EPA.AutoService.Criteria;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Period.GenerationEvaluation;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class SemiAnnualService : BaseService, ISemiAnnualService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IEvaluationService evaluationService;
        private readonly IPeriodService periodService;

        public SemiAnnualService()
            : this(null, null)
        {
        }

        public SemiAnnualService(
            IEvaluationService evaluationService,
            IPeriodService periodService)
        {
            this.evaluationService = evaluationService ?? new EvaluationService();
            this.periodService = periodService ?? new PeriodService();
        }

        public void CheckSemiAnnualPeriod()
        {
            IEnumerable<int> semiAnnualPeriods = CheckStartSemiAnnual();
            Log.Info("Result of check the start of semi annual period: " + string.Join(",", semiAnnualPeriods));

            if (semiAnnualPeriods.Any())
            {
                AddEmployeeAndGenerateEPASemiAnnual(semiAnnualPeriods);
            }
        }

        public void AddEmployeeAndGenerateEPASemiAnnual(IEnumerable<int> semiAnnualPeriods)
        {
            foreach (int periodId in semiAnnualPeriods)
            {
                var insertEmployeeToSemiAnnualPeriodCriteria = new InsertEmployeeToSemiAnnualPeriodCriteria
                {
                    PeriodId = periodId
                };
                CommandResult insertEmployeeResult = InsertEmployeeToSemiAnnualPeriod(insertEmployeeToSemiAnnualPeriodCriteria);
                string messageResult = insertEmployeeResult.IsSuccessful ? "0" : insertEmployeeResult.Message;
                Log.Info("Result of auto add employee to semi annual period " + periodId + ": " + messageResult);

                var generateEvaluationCriteria = new AutoGenerateEvaluationCriteria
                {
                    PeriodId = periodId
                };
                CommandResult generateEvaluationResult = periodService.GenerateEvaluation(generateEvaluationCriteria);
                messageResult = generateEvaluationResult.IsSuccessful ? "0" : generateEvaluationResult.Message;
                Log.Info("Result of auto generate EPA to all employees of current semi annual period " + periodId + ": " + messageResult);

                if (AppSetting.SendMailAllWhenPeriodStarted)
                {
                    Task.Factory.StartNew(() =>
                    {
                        evaluationService.GetAndSendEmailToAllRelatedEmployee(periodId, string.Empty, null, (int)MailMode.Start);
                    });
                }
            }
        }

        public IEnumerable<int> CheckStartSemiAnnual()
        {
            var criteria = new GetSemiAnnualPeriodStartedCriteria();
            IEnumerable<int> startedPeriods = new List<int>();

            if (criteria != null && criteria.IsValid())
            {
                startedPeriods = GetData<int>(criteria);
            }

            return startedPeriods;
        }

        public CommandResult InsertEmployeeToSemiAnnualPeriod(InsertEmployeeToSemiAnnualPeriodCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            return commandResult;
        }
    }
}