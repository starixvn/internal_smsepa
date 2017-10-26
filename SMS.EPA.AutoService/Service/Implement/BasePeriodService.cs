namespace SMS.EPA.AutoService.Service.Implement
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using EPA.Criteria;
    using Fanex.Data.Repository;
    using log4net;
    using Model;
    using Services.Implement;
    using Services.Interfaces;

    public class BasePeriodService : BaseService
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        protected readonly IPeriodMailService PeriodMailService;
        protected readonly IPeriodService PeriodService;

        public BasePeriodService() : this(null, null)
        {
        }

        public BasePeriodService(
            IPeriodService periodService,
            IPeriodMailService periodMailService)
        {
            PeriodService = periodService ?? new PeriodService();
            PeriodMailService = periodMailService ?? new PeriodMailService();
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
                commandResult = Repository.ExecuteNonQuery(criteria);
            }

            if (!commandResult.IsSuccessful)
            {
                Log.Info($"{messageExplanation} {periodId}: {commandResult.Message}");
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
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = PeriodService.GenerateEvaluation(criteria);
            }

            if (!commandResult.IsSuccessful)
            {
                Log.Info($"{messageExplanation} {periodId}: {commandResult.Message}");
            }
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

            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = PeriodService.GenerateEvaluation(criteria);
            }

            if (!commandResult.IsSuccessful)
            {
                Log.Info($"{messageExplanation} {employeePeriod.PeriodId}: {commandResult.Message}");
            }
        }

        protected IEnumerable<AutoEmployeePeriodModel> GetEmployeeEvaluation(ICriteria criteria)
        {
            IEnumerable<AutoEmployeePeriodModel> employeePeriods = new List<AutoEmployeePeriodModel>();

            if (criteria.IsValid())
            {
                employeePeriods = GetData<AutoEmployeePeriodModel>(criteria);
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