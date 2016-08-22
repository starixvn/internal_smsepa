namespace SMS.EPA.AutoGenerateEvaluation.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading.Tasks;
    using log4net;
    using SMS.EPA.AutoGenerateEvaluation.Criteria;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.Period.GenerationEvaluation;
    using SMS.EPA.Model.Period;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class PromotionService : BaseService, IPromotionService
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly IEvaluationService evaluationService;
        private readonly IPeriodService periodService;

        public PromotionService()
            : this(null, null)
        {
        }

        public PromotionService(
            IEvaluationService evaluationService,
            IPeriodService periodService)
        {
            this.evaluationService = evaluationService ?? new EvaluationService();
            this.periodService = periodService ?? new PeriodService();
        }

        public void CheckPromotionPeriod()
        {
            Log.Info("Check the start of promotion evaluation: started at " + DateTime.Now);
            IEnumerable<AutoEmployeePeriodModel> employeePeriods = GetEmployeePromotionEvaluation();
            Log.Info("Result of check the start of promotion evaluation: ended at  " + DateTime.Now);

            if (employeePeriods != null && employeePeriods.Any())
            {
                GenerateEPAPromotion(employeePeriods);
            }
        }

        private void GenerateEPAPromotion(IEnumerable<AutoEmployeePeriodModel> employeePeriods)
        {
            foreach (AutoEmployeePeriodModel period in employeePeriods)
            {
                Log.Info("Auto generate EPA to all employees of current promotion period " + period.PeriodId + ": started at " + DateTime.Now);
                var generateEvaluationCriteria = new AutoGenerateEvaluationCriteria
                {
                    PeriodId = period.PeriodId,
                    EmployeeIds = period.EmployeeIds
                };
                CommandResult generateEvaluationResult = periodService.GenerateEvaluation(generateEvaluationCriteria);
                string messageResult = generateEvaluationResult.IsSuccessful ? "0" : generateEvaluationResult.Message;
                Log.Info("Result of auto generate EPA to all employees of current promotion period " + period.PeriodId + ": " + messageResult);

                if (AppSetting.SendMailAllWhenPeriodStarted)
                {
                    Task.Factory.StartNew(() =>
                    {
                        evaluationService.GetAndSendEmailToAllRelatedEmployee(period.PeriodId, period.EmployeeIds, null, (int)MailMode.Start);
                    });
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
                    EmployeeIds = JoinEmployeeId(x.ToList())
                });
            }

            return employeePeriods;
        }

        private string JoinEmployeeId(List<AutoEmployeePeriodModel> employeePeriods)
        {
            string employeeIds = string.Join(",", employeePeriods.Select(x => x.EmployeeId.ToString()));

            return employeeIds;
        }
    }
}