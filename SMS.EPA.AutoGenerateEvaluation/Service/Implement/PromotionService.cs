namespace SMS.EPA.AutoGenerateEvaluation.Service
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using log4net;
    using SMS.EPA.AutoGenerateEvaluation.Criteria;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Period.GenerationEvaluation;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class PromotionService : BaseService, IPromotionService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IPeriodService periodService;
        private readonly IAutoMailService autoMailService;

        public PromotionService()
            : this(null, null)
        {
        }

        public PromotionService(
            IPeriodService periodService,
            IAutoMailService autoMailService)
        {
            this.periodService = periodService ?? new PeriodService();
            this.autoMailService = autoMailService ?? new AutoMailService();
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

            if (criteria != null && criteria.IsValid())
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
                commandResult = periodService.GenerateEvaluation(criteria);
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
                    PeriodModel period = periodService.GetPeriodById(periodId);
                    IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds);

                    if (employeeSettings != null && employeeSettings.Any())
                    {
                        autoMailService.SendStartMail(period, employeeSettings);
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
                employeeSettings = periodService.GetEmployeeSettingByPeriodId(criteria);
            }

            return employeeSettings;
        }
    }
}