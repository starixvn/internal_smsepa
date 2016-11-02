namespace SMS.EPA.AutoService.Service
{
    using System.Collections.Generic;
    using SMS.EPA.AutoService.Criteria;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Services.Implement;
    using SMS.EPA.Services.Interfaces;

    public class EvaluationService : BaseService, IEvaluationService
    {
        private readonly IPeriodService periodService;
        private readonly IAutoMailService autoMailService;
        private readonly ICryptographyService cryptographyService;

        public EvaluationService()
            : this(null, null, null)
        {
        }

        public EvaluationService(
            IPeriodService periodService,
            IAutoMailService autoMailService,
            ICryptographyService cryptographyService)
        {
            this.periodService = periodService ?? new PeriodService();
            this.autoMailService = autoMailService ?? new AutoMailService();
            this.cryptographyService = cryptographyService ?? new CryptographyService();
        }

        public IEnumerable<AutoEmployeePeriodModel> GetPeriodHasReminderProcess(GetPeriodHasReminderProcessCriteria criteria)
        {
            IEnumerable<AutoEmployeePeriodModel> reminderPeriods = new List<AutoEmployeePeriodModel>();

            if (criteria != null && criteria.IsValid())
            {
                reminderPeriods = GetData<AutoEmployeePeriodModel>(criteria);
            }

            return reminderPeriods;
        }

        public IEnumerable<AutoEmployeePeriodModel> CheckEndProcess()
        {
            var criteria = new CheckEndProcessCriteria();
            IEnumerable<AutoEmployeePeriodModel> evaluationProcesses = new List<AutoEmployeePeriodModel>();

            if (criteria.IsValid())
            {
                evaluationProcesses = GetData<AutoEmployeePeriodModel>(criteria);
                DecryptTotalScore(evaluationProcesses);
            }

            return evaluationProcesses;
        }

        public CommandResult UpdateEndProcess(UpdateIsEndProcessCriteria criteria)
        {
            CommandResult comandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                comandResult = Repository.Update(criteria);
            }

            return comandResult;
        }

        public CommandResult UpdateFinalScoreEndEvaluation(UpdateFinalScoreEndEvaluationCriteria criteria)
        {
            CommandResult comandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                comandResult = Repository.Update(criteria);
            }

            return comandResult;
        }

        public int CheckLastestProcessOfPeriod(int periodId)
        {
            var criteria = new CheckLastestProcessOfPeriodCriteria
            {
                PeriodId = periodId
            };
            int lastestProcess = 0;

            if (criteria.IsValid())
            {
                lastestProcess = GetDataEntity<int>(criteria);
            }

            return lastestProcess;
        }

        public CommandResult UpdateTotalScoreOfProcess(UpdateTotalScoreOfProcessCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public CommandResult UpdateTotalScoreOfSemiHasMonthly(UpdateTotalScoreOfSemiHasMonthlyCriteria criteria)
        {
            CommandResult commandResult = new CommandResult();

            if (criteria != null && criteria.IsValid())
            {
                commandResult = Repository.Update(criteria);
            }

            return commandResult;
        }

        public void GetAndSendEmailToAllRelatedEmployee(int periodId, string employeeIds, int? processId, int mailMode)
        {
            PeriodModel period = periodService.GetPeriodById(periodId);
            IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds, processId);

            switch (mailMode)
            {
                case (int)MailMode.Start:
                    autoMailService.SendStartMail(period, employeeSettings);
                    break;

                case (int)MailMode.Reminder:
                    autoMailService.SendReminderMail(period, employeeSettings);
                    break;

                case (int)MailMode.EndPeriod:
                    autoMailService.SendEndEvaluationMailToHumanResourceByPeriod(period, employeeSettings);
                    break;
            }
        }

        private IEnumerable<PeriodFlowModel> GetEmployeeSettings(int periodId, string employeeIds, int? processId)
        {
            var criteria = new GetEmployeeSettingByPeriodIdCriteria
            {
                PeriodId = periodId,
                EmployeeIds = employeeIds,
                ProcessId = processId
            };

            IEnumerable<PeriodFlowModel> employeeSettings = periodService.GetEmployeeSettingByPeriodId(criteria);

            return employeeSettings;
        }

        private void DecryptTotalScore(IEnumerable<AutoEmployeePeriodModel> evaluationProcesses)
        {
            foreach (AutoEmployeePeriodModel evaluationProcesse in evaluationProcesses)
            {
                evaluationProcesse.TotalScore = cryptographyService.Decrypt(evaluationProcesse.TotalScore);
            }
        }
    }
}