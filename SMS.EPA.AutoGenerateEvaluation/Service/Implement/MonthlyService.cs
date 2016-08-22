namespace SMS.EPA.AutoGenerateEvaluation.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using log4net;
    using SMS.EPA.AutoGenerateEvaluation.Criteria;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Period;
    using SMS.EPA.Criteria.Period.GenerationEvaluation;
    using SMS.EPA.Model.FlowManagement;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class MonthlyService : BaseService, IMonthlyService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IEvaluationService evaluationService;
        private readonly IPeriodService periodService;

        public MonthlyService()
            : this(null, null)
        {
        }

        public MonthlyService(
            IEvaluationService evaluationService,
            IPeriodService periodService)
        {
            this.evaluationService = evaluationService ?? new EvaluationService();
            this.periodService = periodService ?? new PeriodService();
        }

        public void CreateMonthlyPeriod()
        {
            var getSettingCriteria = new GetEvaluationGlobalSettingByTargetTypeIdCriteria()
            {
                TargetTypeId = (int)TargetType.Monthly
            };

            IEnumerable<FlowModel> processSettings = periodService.GetEvaluationGlobalSettingByTargetTypeId(getSettingCriteria);

            if (processSettings != null && processSettings.Any())
            {
                DateTime today = DateTime.Today;
                int startDayCreateMonthlyPeriod = AppSetting.StartDayCreateMonthlyPeriod;

                if (today.Day.Equals(startDayCreateMonthlyPeriod))
                {
                    var criteria = new AutoCreateMonthlyPeriodCriteria()
                    {
                        Name = today.AddMonths(1).ToString(Constants.FormatMonthlyPeriodName),
                        StartDate = today,
                        EndDate = today.AddDays(processSettings.Sum(p => p.Duration))
                    };

                    Log.Info("Create monthly period: started at " + DateTime.Now);
                    CommandResult commandResult = periodService.AutoCreateMonthlyPeriod(criteria);
                    Log.Info("Result of Create monthly period: " + (commandResult.IsSuccessful ? "0" : commandResult.Message));
                }
            }
        }

        public void CheckMonthlyPeriod()
        {
            Log.Info("Check the start of monthly period: started at " + DateTime.Now);
            IEnumerable<int> monthlyPeriods = GetMonthlyPeriodStarted();
            Log.Info("Result of check the start of monthly period: " + string.Join(",", monthlyPeriods));

            if (monthlyPeriods.Any())
            {
                AddEmployeeAndGenerateEPAMonthly(monthlyPeriods);
            }
        }

        private IEnumerable<int> GetMonthlyPeriodStarted()
        {
            var criteria = new GetMonthlyPeriodStartedCriteria();
            IEnumerable<int> startedPeriods = new List<int>();

            if (criteria != null && criteria.IsValid())
            {
                startedPeriods = GetData<int>(criteria);
            }

            return startedPeriods;
        }

        private void AddEmployeeAndGenerateEPAMonthly(IEnumerable<int> monthlyPeriods)
        {
            foreach (int periodId in monthlyPeriods)
            {
                Log.Info("Auto add employee to monthly period " + periodId + ": started at " + DateTime.Now);
                var insertEmployeeToMonthlyPeriodCriteria = new InsertEmployeeToMonthlyPeriodCriteria
                {
                    PeriodId = periodId
                };
                CommandResult insertEmployeeResult = InsertEmployeeToMonthlyPeriod(insertEmployeeToMonthlyPeriodCriteria);
                string messageResult = insertEmployeeResult.IsSuccessful ? "0" : insertEmployeeResult.Message;
                Log.Info("Result of auto add employee to monthly period " + periodId + ": " + messageResult);

                Log.Info("Auto generate EPA to all employees of current monthly period " + periodId + ": started at " + DateTime.Now);
                var generateEvaluationCriteria = new AutoGenerateEvaluationCriteria
                {
                    PeriodId = periodId
                };
                CommandResult generateEvaluationResult = periodService.GenerateEvaluation(generateEvaluationCriteria);
                messageResult = generateEvaluationResult.IsSuccessful ? "0" : generateEvaluationResult.Message;
                Log.Info("Result of auto generate EPA to all employees of current monthly period " + periodId + ": " + messageResult);

                if (AppSetting.SendMailAllWhenPeriodStarted)
                {
                    Task.Factory.StartNew(() =>
                    {
                        evaluationService.GetAndSendEmailToAllRelatedEmployee(periodId, string.Empty, null, (int)MailMode.Start);
                    });
                }
            }
        }

        private CommandResult InsertEmployeeToMonthlyPeriod(InsertEmployeeToMonthlyPeriodCriteria criteria)
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