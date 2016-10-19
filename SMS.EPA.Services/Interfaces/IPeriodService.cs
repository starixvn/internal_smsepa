namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;

    public interface IPeriodService
    {
        IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria);

        PeriodModel GetPeriodById(int periodId);

        IEnumerable<PeriodModel> GetMonthlyPeriod(GetMonthlyPeriodCriteria criteria);

        IEnumerable<PeriodModel> GetAllMonthlyPeriod(GetAllMonthlyPeriodCriteria criteria);

        CommandResult CreatePeriod(CreatePeriodCriteria criteria);

        CommandResult AutoCreateMonthlyPeriod(AutoCreateMonthlyPeriodCriteria criteria);

        CommandResult UpdateBasicInformationPeriod(UpdateBasicInformationPeriodCriteria criteria);

        CommandResult UpdateStatusPeriod(UpdateStatusPeriodCriteria criteria);

        CommandResult DeletePeriod(DeletePeriodCriteria criteria);

        IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(GetPeriodEmployeeSettingCriteria criteria);

        CommandResult AddEmployeeToPeriod(AddEmployeeToPeriodCriteria criteria);

        CommandResult UpdateEmployeeSettingOfPeriod(UpdateEmployeeSettingOfPeriodCriteria criteria);

        CommandResult UpdateIncludeStatusEmployeePeriod(UpdateIncludeStatusEmployeePeriodCriteria criteria);

        void ExportEmployeeSettingReport(IEnumerable<PeriodEmployeeSettingModel> employeeFlowSettings, string templatePath, string periodName);

        CommandResult GenerateEvaluation(AutoGenerateEvaluationCriteria criteria);

        void SendEmailNoticeAtBegin(int? periodId);

        IEnumerable<AutoEmployeePeriodModel> GetPeriodHasStartedProcess(int? periodId = null);

        IEnumerable<PeriodFlowModel> GetEmployeeSettingByPeriodId(GetEmployeeSettingByPeriodIdCriteria criteria);

        IEnumerable<FlowModel> GetProcessSettingByPeriodId(int periodId);

        IEnumerable<FlowModel> GetEvaluationGlobalSettingByTargetTypeId(int targetTypeId);

        CommandResult UpdateProcessSettingPeriod(UpdateProcessSettingPeriodCriteria criteria);

        bool CheckSelfProcessStarted(CheckSelfProcessStartedCriteria criteria);
    }
}