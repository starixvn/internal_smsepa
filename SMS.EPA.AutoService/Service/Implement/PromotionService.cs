namespace SMS.EPA.AutoService.Service
{
    using Core.Model;
    using Core.Services;
    using Criteria;
    using EPA.Criteria;
    using log4net;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;

    public class PromotionService : BaseService, IPromotionService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IPeriodService _periodService;
        private readonly IAutoMailService _autoMailService;

        public PromotionService()
            : this(null, null)
        {
        }

        public PromotionService(
            IPeriodService periodService,
            IAutoMailService autoMailService)
        {
            _periodService = periodService ?? new PeriodService();
            _autoMailService = autoMailService ?? new AutoMailService();
        }

        public void CheckPromotionPeriod()
        {
            IEnumerable<AutoEmployeePeriodModel> employeePeriods = GetEmployeePromotionEvaluation();

            if (employeePeriods != null && employeePeriods.Any())
            {
                foreach (AutoEmployeePeriodModel employeePeriod in employeePeriods)
                {
                    if (!string.IsNullOrWhiteSpace(employeePeriod.EmployeeIds) && employeePeriod.PeriodId > 0)
                    {
                        GenerateEPA(employeePeriod);
                        SendStartEmail(employeePeriod.PeriodId, employeePeriod.EmployeeIds);
                    }
                }
            }
        }

        private IEnumerable<AutoEmployeePeriodModel> GetEmployeePromotionEvaluation()
        {
            var criteria = new GetEmployeePromotionEvaluationCriteria();
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

        private void GenerateEPA(AutoEmployeePeriodModel employeePeriod)
        {
            var criteria = new AutoGenerateEvaluationCriteria
            {
                PeriodId = employeePeriod.PeriodId,
                EmployeeIds = employeePeriod.EmployeeIds
            };
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = _periodService.GenerateEvaluation(criteria);
            }

            string messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
            Log.Info("Result of auto generate EPA to all employees of current promotion period " + employeePeriod.PeriodId + ": " + messageResult);
        }

        private void SendStartEmail(int periodId, string employeeIds)
        {
            if (AppSetting.SendMailAllWhenPeriodStarted)
            {
                Task.Factory.StartNew(() =>
                {
                    PeriodModel period = _periodService.GetPeriodById(periodId);
                    IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds);

                    if (employeeSettings != null && employeeSettings.Any())
                    {
                        _autoMailService.SendStartMail(period, employeeSettings);
                    }
                });
            }
        }

        private IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId, string employeeIds)
        {
            IEnumerable<PeriodFlowModel> employeeSettings = new List<PeriodFlowModel>();
            var criteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds
            };

            if (criteria.IsValid())
            {
                employeeSettings = _periodService.GetEmployeeSettingByPeriodId(criteria);
            }

            return employeeSettings;
        }
    }
}