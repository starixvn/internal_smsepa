namespace SMS.EPA.AutoService.Service
{
    using Core.Enumerations;
    using Core.Model;
    using Core.Services;
    using Criteria;
    using EPA.Criteria;
    using Model;
    using Services.Implement;
    using Services.Interfaces;
    using System.Collections.Generic;

    public class EvaluationService : BaseService, IEvaluationService
    {
        private readonly IPeriodService _periodService;
        private readonly IPeriodMailService _periodMailService;
        private readonly ICryptographyService _cryptographyService;

        public EvaluationService()
            : this(null, null, null)
        {
        }

        public EvaluationService(
            IPeriodService periodService,
            IPeriodMailService periodMailService,
            ICryptographyService cryptographyService)
        {
            _periodService = periodService ?? new PeriodService();
            _periodMailService = periodMailService ?? new PeriodMailService();
            _cryptographyService = cryptographyService ?? new CryptographyService();
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
            PeriodModel period = _periodService.GetPeriodById(periodId);
            IEnumerable<PeriodFlowModel> employeeSettings = GetEmployeeSettings(periodId, employeeIds, processId);

            switch (mailMode)
            {
                case (int)MailMode.Start:
                    _periodMailService.SendReminderMail(period, employeeSettings, (int)MailMode.Start);
                    break;

                case (int)MailMode.Reminder:
                    _periodMailService.SendReminderMail(period, employeeSettings, (int)MailMode.Reminder);
                    break;

                case (int)MailMode.EndPeriod:
                    _periodMailService.SendEndEvaluationMailToHumanResourceByPeriod(period, employeeSettings);
                    break;
                default:
                    return;
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

            IEnumerable<PeriodFlowModel> employeeSettings = _periodService.GetEmployeeSettingByPeriodId(criteria);

            return employeeSettings;
        }

        private void DecryptTotalScore(IEnumerable<AutoEmployeePeriodModel> evaluationProcesses)
        {
            foreach (AutoEmployeePeriodModel evaluationProcesse in evaluationProcesses)
            {
                evaluationProcesse.TotalScore = _cryptographyService.Decrypt(evaluationProcesse.TotalScore);
            }
        }
    }
}