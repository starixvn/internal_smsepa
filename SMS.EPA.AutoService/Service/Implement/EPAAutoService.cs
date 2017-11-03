namespace SMS.EPA.AutoService.Service.Implement
{
    using System;
    using System.Threading.Tasks;
    using Fanex.Logging;
    using Interfaces;
    using Service;

    public class EPAAutoService : IEPAAutoService
    {
        private readonly IMonthlyService monthlyService;
        private readonly ISemiAnnualService semiAnnualService;
        private readonly IProbationService probationService;
        private readonly IPromotionService promotionService;
        private readonly IProcessService processService;
        private readonly IAutoMailService autoMailService;
        private readonly IEvaluationPeriodService evaluationPeriodService;

        public EPAAutoService()
            : this(null, null, null, null, null, null, null)
        {
        }

        public EPAAutoService(
            IMonthlyService monthlyService,
            ISemiAnnualService semiAnnualService,
            IProbationService probationService,
            IPromotionService promotionService,
            IProcessService processService,
            IAutoMailService autoMailService,
            IEvaluationPeriodService evaluationPeriodService)
        {
            this.monthlyService = monthlyService ?? new MonthlyService();
            this.semiAnnualService = semiAnnualService ?? new SemiAnnualService();
            this.probationService = probationService ?? new ProbationService();
            this.promotionService = promotionService ?? new PromotionService();
            this.processService = processService ?? new ProcessService();
            this.autoMailService = autoMailService ?? new AutoMailService();
            this.evaluationPeriodService = evaluationPeriodService ?? new EvaluationPeriodService();
        }

        public void Run()
        {
            try
            {
                if (AppSetting.EnableMonthlyPeriod)
                {
                    monthlyService.CreateMonthlyPeriod();
                    monthlyService.CheckMonthlyPeriod();
                }

                semiAnnualService.CheckSemiAnnualPeriod();
                probationService.CheckProbationPeriod();
                promotionService.CheckPromotionPeriod();
                processService.UpdateInfomationOfProcess();

                Task.WaitAll(new[] {
                    Task.Factory.StartNew(() => { autoMailService.CheckAndSendMail(); }),
                    Task.Factory.StartNew(() => { evaluationPeriodService.CheckEndEvaluationPeriod(); })
                });
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(ex.Message))
                {
                    LogService.Error(ex.Message, ex);
                }

                if (ex.InnerException != null)
                {
                    LogService.Error(ex.InnerException.Message, ex);
                }
            }
        }
    }
}