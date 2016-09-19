namespace SMS.EPA.AutoGenerateEvaluation.Service
{
    using System.Reflection;
    using System.Threading.Tasks;
    using log4net;
    using SMS.EPA.AutoGenerateEvaluation.Criteria;
    using SMS.EPA.AutoGenerateEvaluation.Repository;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Period.GenerationEvaluation;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class ProbationService : BaseService, IProbationService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IEvaluationRepository generateEvaluationRepository;
        private readonly IEvaluationService evaluationService;
        private readonly IPeriodService periodService;

        public ProbationService()
            : this(null, null, null)
        {
        }

        public ProbationService(
            IEvaluationRepository generationEvaluationRepository,
            IEvaluationService evaluationService,
            IPeriodService periodService)
        {
            this.generateEvaluationRepository = generationEvaluationRepository ?? new EvaluationRepository();
            this.evaluationService = evaluationService ?? new EvaluationService();
            this.periodService = periodService ?? new PeriodService();
        }

        public void CheckProbationPeriod()
        {
            AutoEmployeePeriodModel employeePeriod = GetEmployeeProbationEvaluation();
            Log.Info("Result of check the start of probation evaluation: employee: " + employeePeriod.EmployeeIds + "; periodId: " + employeePeriod.PeriodId);

            if (!string.IsNullOrWhiteSpace(employeePeriod.EmployeeIds) && employeePeriod.PeriodId > 0)
            {
                AddEmployeeAndGenerateEPAProbation(employeePeriod);
            }
        }

        private void AddEmployeeAndGenerateEPAProbation(AutoEmployeePeriodModel employeePeriod)
        {
            var insertEmployeeCriteria = new InsertEmployeeToProbationPeriodCriteria
            {
                PeriodId = employeePeriod.PeriodId,
                EmployeeIds = employeePeriod.EmployeeIds
            };
            CommandResult insertEmployeeResult = InsertEmployeeToProbationPeriod(insertEmployeeCriteria);
            string messageResult = insertEmployeeResult.IsSuccessful ? "0" : insertEmployeeResult.Message;
            Log.Info("Result of auto add employee to probation period " + employeePeriod.PeriodId + ": " + messageResult);

            var generateEvaluationCriteria = new AutoGenerateEvaluationCriteria
            {
                PeriodId = employeePeriod.PeriodId,
                EmployeeIds = employeePeriod.EmployeeIds
            };
            CommandResult generateEvaluationResult = periodService.GenerateEvaluation(generateEvaluationCriteria);
            Log.Info("Result of auto generate EPA to all employees of current probation period " + employeePeriod.PeriodId + ": " + messageResult);

            if (AppSetting.SendMailAllWhenPeriodStarted)
            {
                Task.Factory.StartNew(() =>
                {
                    evaluationService.GetAndSendEmailToAllRelatedEmployee(employeePeriod.PeriodId, employeePeriod.EmployeeIds, null, (int)MailMode.Start);
                });
            }
        }

        private AutoEmployeePeriodModel GetEmployeeProbationEvaluation()
        {
            var criteria = new GetEmployeeProbationEvaluationCriteria();
            AutoEmployeePeriodModel employeePeriod = new AutoEmployeePeriodModel();

            if (criteria != null && criteria.IsValid())
            {
                employeePeriod = generateEvaluationRepository.GetEmployeeProbationEvaluation(criteria);
            }

            return employeePeriod;
        }

        private CommandResult InsertEmployeeToProbationPeriod(InsertEmployeeToProbationPeriodCriteria criteria)
        {
            CommandResult comandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                comandResult = Repository.Insert(criteria);
            }

            return comandResult;
        }
    }
}