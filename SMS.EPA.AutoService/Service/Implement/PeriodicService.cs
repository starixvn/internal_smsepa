namespace SMS.EPA.AutoService.Service.Implement
{
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using EPA.Criteria;
    using Fanex.Data.Repository;
    using log4net;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public class PeriodicService : BaseService
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected readonly IAutoMailService AutoMailService;
        protected readonly IPeriodService PeriodService;

        public PeriodicService() : this(null, null)
        {
        }

        public PeriodicService(
            IPeriodService periodService,
            IAutoMailService autoMailService)
        {
            PeriodService = periodService ?? new PeriodService();
            AutoMailService = autoMailService ?? new AutoMailService();
        }

        protected IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId)
        {
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();
            var criteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId
            };

            if (criteria.IsValid())
            {
                employeeSettings = PeriodService.GetEmployeeSettingByPeriodId(criteria);
            }

            return employeeSettings;
        }

        protected void InsertEmployeeToPeriod(int periodId, ICriteria criteria, string messageExplanation)
        {
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            string messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
            Log.Info($"{messageExplanation} {periodId}: {messageResult}");
        }

        protected void SendStartEmail(int periodId)
        {
            if (AppSetting.SendMailAllWhenPeriodStarted)
            {
                Task.Factory.StartNew(() =>
                {
                    PeriodModel period = PeriodService.GetPeriodById(periodId);
                    IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId);

                    if (employeeSettings != null && employeeSettings.Any())
                    {
                        AutoMailService.SendReminderMail(period, employeeSettings, (int)MailMode.Start);
                    }
                });
            }
        }

        protected void GenerateEPA(int periodId, string messageExplanation)
        {
            var criteria = new AutoGenerateEvaluationCriteria
            {
                PeriodId = periodId
            };
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = PeriodService.GenerateEvaluation(criteria);
            }

            string messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
            Log.Info(
                $"{messageExplanation} {periodId}: {messageResult}");
        }

        protected IEnumerable<int> GetStartedPeriods(ICriteria criteria)
        {
            IEnumerable<int> startedPeriods = new List<int>();

            if (criteria.IsValid())
            {
                startedPeriods = GetData<int>(criteria);
            }

            return startedPeriods;
        }
    }
}
