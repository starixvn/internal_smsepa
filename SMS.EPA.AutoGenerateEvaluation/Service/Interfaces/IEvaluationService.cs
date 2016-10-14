namespace SMS.EPA.AutoGenerateEvaluation.Service
{
    using System.Collections.Generic;
    using SMS.EPA.AutoGenerateEvaluation.Criteria;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.Period;

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