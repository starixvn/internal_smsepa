namespace SMS.EPA.AutoService.Service.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using EPA.Criteria;
    using Fanex.Data.Repository;
    using Model;
    using Services.Implement;
    using Services.Interfaces;

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
                commandResult = BaseService.ExecuteNonQuery(criteria);
            }

            if (!commandResult.IsSuccessful)
            {
                logger.Error(string.Format("{0} {1}: {2}", messageExplanation, periodId, commandResult.Message));
            }
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
                        PeriodMailService.SendReminderMail(period, employeeSettings, (int)MailMode.Start);
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
            CommandResult commandResult = BaseService.ExecuteNonQuery(criteria);

            if (!commandResult.IsSuccessful)
            {
                logger.Error(string.Format("{0} {1}: {2}", messageExplanation, periodId, commandResult.Message));
            }
        }

        protected IEnumerable<int> GetStartedPeriods(ICriteria criteria)
        {
            IEnumerable<int> startedPeriods = new List<int>();

            if (criteria.IsValid())
            {
                startedPeriods = BaseService.GetData<int>(criteria);
            }

            return startedPeriods;
        }

        protected IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId, string employeeIds)
        {
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();
            var criteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds
            };

            if (criteria.IsValid())
            {
                employeeSettings = PeriodService.GetEmployeeSettingByPeriodId(criteria);
            }

            return employeeSettings;
        }

        protected void SendStartEmail(int periodId, string employeeIds)
        {
            if (AppSetting.SendMailAllWhenPeriodStarted)
            {
                Task.Factory.StartNew(() =>
                {
                    PeriodModel period = PeriodService.GetPeriodById(periodId);
                    IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds);

                    if (employeeSettings != null && employeeSettings.Any())
                    {
                        PeriodMailService.SendReminderMail(period, employeeSettings, (int)MailMode.Start);
                    }
                });
            }
        }

        protected void GenerateEPA(AutoEmployeePeriodModel employeePeriod, string messageExplanation)
        {
            var criteria = new AutoGenerateEvaluationCriteria
            {
                PeriodId = employeePeriod.PeriodId,
                EmployeeIds = employeePeriod.EmployeeIds
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
    }
}