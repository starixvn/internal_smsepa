namespace SMS.EPA.AutoService.Service
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using log4net;
    using SMS.EPA.AutoService.Criteria;
    using SMS.EPA.AutoService.Repository;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class ProbationService : BaseService, IProbationService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IEvaluationRepository evaluationRepository;
        private readonly IPeriodService periodService;
        private readonly IAutoMailService autoMailService;

        public ProbationService()
            : this(null, null, null)
        {
        }

        public ProbationService(
            IEvaluationRepository evaluationRepository,
            IPeriodService periodService,
            IAutoMailService autoMailService)
        {
            this.evaluationRepository = evaluationRepository ?? new EvaluationRepository();
            this.periodService = periodService ?? new PeriodService();
            this.autoMailService = autoMailService ?? new AutoMailService();
        }

        public void CheckProbationPeriod()
        {
            AutoEmployeePeriodModel employeePeriod = GetEmployeeProbationEvaluation();
            Log.Info("Result of check the start of probation evaluation: employee: " + employeePeriod.EmployeeIds + "; periodId: " + employeePeriod.PeriodId);

            if (!string.IsNullOrWhiteSpace(employeePeriod.EmployeeIds) && employeePeriod.PeriodId > 0)
            {
                AddEmployeeToEPA(employeePeriod);
                GenerateEPA(employeePeriod);
                SendStartEmail(employeePeriod.PeriodId, employeePeriod.EmployeeIds);
            }
        }

        private AutoEmployeePeriodModel GetEmployeeProbationEvaluation()
        {
            var criteria = new GetEmployeeProbationEvaluationCriteria();
            AutoEmployeePeriodModel employeePeriod = new AutoEmployeePeriodModel();

            if (criteria.IsValid())
            {
                employeePeriod = evaluationRepository.GetEmployeeProbationEvaluation(criteria);
            }

            return employeePeriod;
        }

        private void AddEmployeeToEPA(AutoEmployeePeriodModel employeePeriod)
        {
            var criteria = new InsertEmployeeToProbationPeriodCriteria
            {
                PeriodId = employeePeriod.PeriodId,
                EmployeeIds = employeePeriod.EmployeeIds
            };
            var comandResult = new CommandResult();

            if (criteria.IsValid())
            {
                comandResult = Repository.Insert(criteria);
            }

            string messageResult = comandResult.IsSuccessful ? "0" : comandResult.Message;
            Log.Info("Result of auto add employee to probation period " + employeePeriod.PeriodId + ": " + messageResult);
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
            Log.Info("Result of auto generate EPA to all employees of current probation period " + employeePeriod.PeriodId + ": " + messageResult);
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