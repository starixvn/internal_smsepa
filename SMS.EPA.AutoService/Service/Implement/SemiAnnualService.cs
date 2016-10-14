namespace SMS.EPA.AutoService.Service
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using log4net;
    using SMS.EPA.AutoService.Criteria;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Period.GenerationEvaluation;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class SemiAnnualService : BaseService, ISemiAnnualService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IPeriodService periodService;
        private readonly IAutoMailService autoMailService;

        public SemiAnnualService()
            : this(null, null)
        {
        }

        public SemiAnnualService(
            IPeriodService periodService,
            IAutoMailService autoMailService)
        {
            this.periodService = periodService ?? new PeriodService();
            this.autoMailService = autoMailService ?? new AutoMailService();
        }

        public void CheckSemiAnnualPeriod()
        {
            IEnumerable<int> semiAnnualPeriods = CheckStartSemiAnnual();
            Log.Info("Result of check the start of semi annual period: " + string.Join(",", semiAnnualPeriods));

            if (semiAnnualPeriods != null && semiAnnualPeriods.Any())
            {
                foreach (int periodId in semiAnnualPeriods)
                {
                    InsertEmployeeToPeriod(periodId);
                    GenerateEPA(periodId);
                    SendStartEmail(periodId);
                }
            }
        }

        private IEnumerable<int> CheckStartSemiAnnual()
        {
            var criteria = new GetSemiAnnualPeriodStartedCriteria();
            IEnumerable<int> startedPeriods = new List<int>();

            if (criteria.IsValid())
            {
                startedPeriods = GetData<int>(criteria);
            }

            return startedPeriods;
        }

        private void InsertEmployeeToPeriod(int periodId)
        {
            var criteria = new InsertEmployeeToSemiAnnualPeriodCriteria
            {
                PeriodId = periodId
            };
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.Insert(criteria);
            }

            string messageResult = commandResult.IsSuccessful ? "0" : commandResult.Message;
            Log.Info("Result of auto add employee to semi annual period " + periodId + ": " + messageResult);
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
            Log.Info("Result of auto generate EPA to all employees of current semi annual period " + periodId + ": " + messageResult);
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