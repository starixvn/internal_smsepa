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
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class MonthlyService : BaseService, IMonthlyService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IPeriodService periodService;
        private readonly IAutoMailService autoMailService;

        public MonthlyService()
            : this(null, null)
        {
        }

        public MonthlyService(
            IPeriodService periodService,
            IAutoMailService autoMailService)
        {
            this.periodService = periodService ?? new PeriodService();
            this.autoMailService = autoMailService ?? new AutoMailService();
        }

        public void CreateMonthlyPeriod()
        {
            IEnumerable<FlowModel> processSettings = periodService.GetEvaluationGlobalSettingByTargetTypeId((int)TargetTypes.Monthly);

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

                    CommandResult commandResult = periodService.AutoCreateMonthlyPeriod(criteria);
                    Log.Info("Result of Create monthly period: " + (commandResult.IsSuccessful ? "0" : commandResult.Message));
                }
            }
        }

        public void CheckMonthlyPeriod()
        {
            IEnumerable<int> monthlyPeriods = GetMonthlyPeriodStarted();
            Log.Info("Result of check the start of monthly period: " + string.Join(",", monthlyPeriods));

            if (monthlyPeriods != null && monthlyPeriods.Any())
            {
                foreach (int periodId in monthlyPeriods)
                {
                    InsertEmployeeToPeriod(periodId);
                    GenerateEPA(periodId);
                    SendStartEmail(periodId);
                }
            }
        }

        private IEnumerable<int> GetMonthlyPeriodStarted()
        {
            var criteria = new GetMonthlyPeriodStartedCriteria();
            IEnumerable<int> startedPeriods = new List<int>();

            if (criteria.IsValid())
            {
                startedPeriods = GetData<int>(criteria);
            }

            return startedPeriods;
        }

        private void InsertEmployeeToPeriod(int periodId)
        {
            var criteria = new InsertEmployeeToMonthlyPeriodCriteria
            {
                PeriodId = periodId
            };
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            string messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
            Log.Info("Result of auto add employee to monthly period " + periodId + ": " + messageResult);
        }

        private void GenerateEPA(int periodId)
        {
            var criteria = new AutoGenerateEvaluationCriteria
            {
                PeriodId = periodId
            };
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = periodService.GenerateEvaluation(criteria);
            }

            string messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
            Log.Info("Result of auto generate EPA to all employees of current monthly period " + periodId + ": " + messageResult);
        }

        private void SendStartEmail(int periodId)
        {
            if (AppSetting.SendMailAllWhenPeriodStarted)
            {
                Task.Factory.StartNew(() =>
                {
                    PeriodModel period = periodService.GetPeriodById(periodId);
                    IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId);

                    if (employeeSettings != null && employeeSettings.Any())
                    {
                        autoMailService.SendStartMail(period, employeeSettings);
                    }
                });
            }
        }

        private IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId)
        {
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();
            var criteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId
            };

            if (criteria.IsValid())
            {
                employeeSettings = periodService.GetEmployeeSettingByPeriodId(criteria);
            }

            return employeeSettings;
        }
    }
}