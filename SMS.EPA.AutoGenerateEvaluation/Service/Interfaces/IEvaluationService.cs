namespace SMS.EPA.AutoGenerateEvaluation.Service.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.AutoGenerateEvaluation.Criteria;
    using SMS.EPA.AutoGenerateEvaluation.Criteria.Monthly;
    using SMS.EPA.AutoGenerateEvaluation.Models;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model.Period;

    public interface IEvaluationService
    {
        IEnumerable<int> CheckStartSemiAnnual(GetSemiAnnualPeriodStartedCriteria criteria);

        CommandResult InsertEmployeeToSemiAnnualPeriod(InsertEmployeeToSemiAnnualPeriodCriteria criteria);

        AutoEmployeePeriodModel GetEmployeeProbationEvaluation(GetEmployeeProbationEvaluationCriteria criteria);

        CommandResult InsertEmployeeToProbationPeriod(InsertEmployeeToProbationPeriodCriteria criteria);

        IEnumerable<AutoEmployeePeriodModel> GetPeriodHasReminderProcess(GetPeriodHasReminderProcessCriteria criteria);

        IEnumerable<AutoEmployeePeriodModel> GetEmployeePromotionEvaluation(GetEmployeePromotionEvaluationCriteria criteria);

        IEnumerable<AutoEmployeePeriodModel> CheckEndProcess(CheckEndProcessCriteria criteria);

        CommandResult UpdateEndProcess(UpdateIsEndProcessCriteria criteria);

        CommandResult UpdateFinalScoreEndEvaluation(UpdateFinalScoreEndEvaluationCriteria criteria);

        int CheckLastestProcessOfPeriod(CheckLastestProcessOfPeriodCriteria criteria);

        CommandResult UpdateTotalScoreOfProcess(UpdateTotalScoreOfProcessCriteria criteria);

        CommandResult UpdateTotalScoreOfSemiHasMonthly(UpdateTotalScoreOfSemiHasMonthlyCriteria criteria);

        #region Monthly

        IEnumerable<int> GetMonthlyPeriodStarted(GetMonthlyPeriodStartedCriteria criteria);

        CommandResult InsertEmployeeToMonthlyPeriod(InsertEmployeeToMonthlyPeriodCriteria criteria);

        #endregion Monthly
    }
}