namespace SMS.EPA.AutoService.Service.Implement
{
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

    public class StaticPeriodService : BaseService
    {
        protected static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        protected readonly IPeriodService PeriodService;
        protected readonly IAutoMailService AutoMailService;

        public StaticPeriodService()
            : this(null, null)
        {
        }

        public StaticPeriodService(
            IPeriodService periodService,
            IAutoMailService autoMailService)
        {
            PeriodService = periodService ?? new PeriodService();
            AutoMailService = autoMailService ?? new AutoMailService();
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
                        AutoMailService.SendStartMail(period, employeeSettings);
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

            string messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
            Log.Info($"{messageExplanation}{employeePeriod.PeriodId}: {messageResult}");
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
