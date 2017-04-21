namespace SMS.EPA.AutoService.Service
{
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using EPA.Criteria;
    using log4net;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public class MonthlyService : BaseService, IMonthlyService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IPeriodService _periodService;
        private readonly IAutoMailService _autoMailService;

        public MonthlyService()
            : this(null, null)
        {
        }

        public MonthlyService(
            IPeriodService periodService,
            IAutoMailService autoMailService)
        {
            _periodService = periodService ?? new PeriodService();
            _autoMailService = autoMailService ?? new AutoMailService();
        }

        public void CreateMonthlyPeriod()
        {
            IEnumerable<FlowModel> processSettings = _periodService.GetEvaluationGlobalSettingByTargetTypeId((int)TargetType.Monthly);

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

                    CommandResult commandResult = _periodService.AutoCreateMonthlyPeriod(criteria);
                    Log.Info("Result of Create monthly period: " + (commandResult.IsSuccessful ? "0" : commandResult.Message));
                }
            }
        }

        public void CheckMonthlyPeriod()
        {
            IEnumerable<int> monthlyPeriods = GetMonthlyPeriodStarted();
            Log.Info("Result of check the start of monthly period: " + string.Join(",", monthlyPeriods));

            if (monthlyPeriods.Any())
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
                commandResult = _periodService.GenerateEvaluation(criteria);
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
                    PeriodModel period = _periodService.GetPeriodById(periodId);
                    IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId);

                    if (employeeSettings != null && employeeSettings.Any())
                    {
                        _autoMailService.SendStartMail(period, employeeSettings);
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
                employeeSettings = _periodService.GetEmployeeSettingByPeriodId(criteria);
            }

            return employeeSettings;
        }
    }
}