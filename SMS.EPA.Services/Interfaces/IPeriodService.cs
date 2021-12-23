namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Model;
    using SMS.EPA.Model.AutoService;

    public interface IPeriodService
    {
        IEnumerable<PeriodModel> GetPeriod(GetPeriodCriteria criteria);

        PeriodModel GetPeriodById(int periodId);

        IEnumerable<PeriodEmployeeSettingModel> GetPeriodEmployeeSettingById(GetPeriodEmployeeSettingCriteria criteria);

        CommandResult UpdateEmployeeSettingOfPeriod(UpdateEmployeeSettingOfPeriodCriteria criteria);

        void SendEmailNoticeAtBegin(int? periodId = null);

        IEnumerable<EmployeeSettingToSendMail> GetEmployeeSettingToSendMail(
            int? periodId = null,
            byte? processId = null,
            byte? mailMode = null,
            string employeeIds = null,
            bool? isEvaluated = null);
    }
}