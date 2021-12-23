namespace SMS.EPA.AutoService.Service.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Model;
    using Core.Services;
    using EPA.Criteria;
    using Fanex.Data.Repository;
    using Model;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.AutoService;
    using SMS.EPA.Services;

    public class BasePeriodService
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(BasePeriodService));
        protected readonly IPeriodMailService PeriodMailService;
        protected readonly IPeriodService PeriodService;
        protected readonly IBaseService BaseService;

        public BasePeriodService() : this(null, null, null)
        {
        }

        public BasePeriodService(
            IPeriodService periodService,
            IPeriodMailService periodMailService,
            IBaseService baseService)
        {
            PeriodService = periodService ?? new PeriodService();
            PeriodMailService = periodMailService ?? new PeriodMailService();
            BaseService = baseService ?? new BaseService();
        }

        protected void InsertEmployeeToPeriod(int periodId, ICriteria criteria, string messageExplanation)
        {
            var commandResult = new CommandResult();
            if (criteria.IsValid())
            {
                commandResult = BaseService.ExecuteNonQuery(criteria);
            }

            if (!commandResult.IsSuccessful)
            {
                logger.Error(string.Format("{0} {1}: {2}", messageExplanation, periodId, commandResult.Message));
            }
        }

        protected void GenerateEPA(int periodId, string messageExplanation)
        {
            var criteria = new AutoGenerateEvaluationCriteria
            {
                PeriodId = periodId
            };
            CommandResult commandResult = BaseService.ExecuteNonQuery(criteria);

            if (!commandResult.IsSuccessful)
            {
                logger.Error(string.Format("{0} {1}: {2}", messageExplanation, periodId, commandResult.Message));
            }
        }

        protected void GenerateEPA(AutoEmployeePeriodModel employeePeriod, string messageExplanation)
        {
            var criteria = new AutoGenerateEvaluationCriteria
            {
                PeriodId = employeePeriod.PeriodId,
                EmployeeIds = employeePeriod.EmployeeIds,
                ExtendedQuestionEnabled = AppSetting.ProbationExtendedQuestionEnabled
            };
            CommandResult commandResult = BaseService.ExecuteNonQuery(criteria);

            if (!commandResult.IsSuccessful)
            {
                logger.Error(string.Format("{0} {1}: {2}", messageExplanation, employeePeriod.PeriodId, commandResult.Message));
            }
        }

        protected IEnumerable<AutoEmployeePeriodModel> GetEmployeeEvaluation(ICriteria criteria)
        {
            IEnumerable<AutoEmployeePeriodModel> employeePeriods = new List<AutoEmployeePeriodModel>();

            if (criteria.IsValid())
            {
                employeePeriods = BaseService.GetData<AutoEmployeePeriodModel>(criteria);
                employeePeriods = employeePeriods.GroupBy(x => x.PeriodId).Select(x => new AutoEmployeePeriodModel
                {
                    PeriodId = x.Key,
                    EmployeeIds = string.Join(",", x.Select(i => i.EmployeeId.ToString()))
                });
            }

            return employeePeriods;
        }

        protected void SendReminderStartEmail(int periodId, string employeeIds = null)
        {
            if (AppSetting.SendMailAllWhenPeriodStarted)
            {
                Task.Factory.StartNew(() =>
                {
                    IEnumerable<EmployeeSettingToSendMail> employeeSettings =
                        PeriodService.GetEmployeeSettingToSendMail(periodId, mailMode: (byte)MailMode.Start, employeeIds: employeeIds);
                    if (employeeSettings != null && employeeSettings.Any())
                    {
                        PeriodMailService.SendReminderMail(employeeSettings, MailMode.Start);
                    }
                });
            }
        }
    }
}