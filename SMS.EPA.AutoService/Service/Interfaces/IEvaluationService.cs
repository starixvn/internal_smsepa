namespace SMS.EPA.AutoService.Service
{
    using Core.Model;
    using Criteria;
    using Model;
    using System.Collections.Generic;

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