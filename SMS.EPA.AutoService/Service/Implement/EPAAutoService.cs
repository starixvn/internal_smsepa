namespace SMS.EPA.AutoService.Service.Implement
{
    using System;
    using System.Threading.Tasks;
    using Interfaces;
    using Service;

    public class EPAAutoService : IEPAAutoService
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(EPAAutoService));
        private readonly IMonthlyService monthlyService;
        private readonly ISemiAnnualService semiAnnualService;
        private readonly IProbationService probationService;
        private readonly IPromotionService promotionService;
        private readonly IProcessService processService;
        private readonly IAutoMailService autoMailService;
        private readonly IEvaluationPeriodService evaluationPeriodService;

        private readonly IJobLevelStandardService jobLevelStandardService;

        public EPAAutoService()
            : this(null, null, null, null, null, null, null, null)
        {
        }

#pragma warning disable S107 // Methods should not have too many parameters
        public EPAAutoService(
            IMonthlyService monthlyService,
            ISemiAnnualService semiAnnualService,
            IProbationService probationService,
            IPromotionService promotionService,
            IProcessService processService,
            IAutoMailService autoMailService,
            IEvaluationPeriodService evaluationPeriodService,
            IJobLevelStandardService jobLevelStandardService)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            this.monthlyService = monthlyService ?? new MonthlyService();
            this.semiAnnualService = semiAnnualService ?? new SemiAnnualService();
            this.probationService = probationService ?? new ProbationService();
            this.promotionService = promotionService ?? new PromotionService();
            this.processService = processService ?? new ProcessService();
            this.autoMailService = autoMailService ?? new AutoMailService();
            this.evaluationPeriodService = evaluationPeriodService ?? new EvaluationPeriodService();
            this.jobLevelStandardService = jobLevelStandardService ?? new JobLevelStandardService();
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

                Task.WhenAll(new[] {
                    Task.Factory.StartNew(() => { autoMailService.CheckAndSendMail(); }),
                    Task.Factory.StartNew(() => { evaluationPeriodService.CheckEndEvaluationPeriod(); }),
                    Task.Factory.StartNew(() => { jobLevelStandardService.SendReminderEmail(); })
                }).Wait();
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrWhiteSpace(ex.Message))
                {
                    logger.Error(ex.Message, ex);
                }

                if (ex.InnerException != null)
                {
                    logger.Error(ex.InnerException.Message, ex);
                }
            }
        }
    }
}