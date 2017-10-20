namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Model;

    public interface IPeriodService
    {
        IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria);

        PeriodModel GetPeriodById(int periodId);

        IEnumerable<PeriodModel> GetMonthlyPeriod(GetMonthlyPeriodCriteria criteria);

        IEnumerable<PeriodModel> GetAllMonthlyPeriod();

        CommandResult AutoCreateMonthlyPeriod(AutoCreateMonthlyPeriodCriteria criteria);

        IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(GetPeriodEmployeeSettingCriteria criteria);

        CommandResult UpdateEmployeeSettingOfPeriod(UpdateEmployeeSettingOfPeriodCriteria criteria);

        CommandResult GenerateEvaluation(AutoGenerateEvaluationCriteria criteria);

        void SendEmailNoticeAtBegin(int? periodId);

        IEnumerable<AutoEmployeePeriodModel> GetPeriodHasStartedProcess(int? periodId = null);

        IEnumerable<PeriodFlowModel> GetEmployeeSettingByPeriodId(GetEmployeeSettingByPeriodIdCriteria criteria);

        IEnumerable<FlowModel> GetProcessSettingByPeriodId(int periodId);

        IEnumerable<FlowModel> GetEvaluationGlobalSettingByTargetTypeId(int targetTypeId);

        bool CheckSelfProcessStarted(CheckSelfProcessStartedCriteria criteria);
    }
}