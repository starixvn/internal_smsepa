namespace SMS.EPA.AutoService.Service
{
    using System.Collections.Generic;
    using SMS.EPA.AutoService.Criteria;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model;

    public interface IEvaluationService
    {
        IEnumerable<AutoEmployeePeriodModel> GetPeriodHasReminderProcess(GetPeriodHasReminderProcessCriteria criteria);

        IEnumerable<AutoEmployeePeriodModel> CheckEndProcess();

        CommandResult UpdateEndProcess(UpdateIsEndProcessCriteria criteria);

        CommandResult UpdateFinalScoreEndEvaluation(UpdateFinalScoreEndEvaluationCriteria criteria);

        int CheckLastestProcessOfPeriod(int periodId);

        CommandResult UpdateTotalScoreOfProcess(UpdateTotalScoreOfProcessCriteria criteria);

        CommandResult UpdateTotalScoreOfSemiHasMonthly(UpdateTotalScoreOfSemiHasMonthlyCriteria criteria);

        void GetAndSendEmailToAllRelatedEmployee(int periodId, string employeeIds, int? processId, int mailMode);
    }
}