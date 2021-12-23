namespace SMS.EPA.Services
{
    using System.Collections.Generic;
    using Model;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Model.AutoService;

    public interface IPeriodMailService
    {
        void SendReminderMail(IEnumerable<EmployeeSettingToSendMail> employeeSettings, MailMode mailMode);

        void SendEndEvaluationMailToSelf(EPAModel evaluationInfo, float? averagePerformanceScore, float disciplineScore);

        void SendEndEvaluationMailToHumanResourceByEmployee(EPAModel evaluationInfo);

        void SendEndEvaluationMailToHumanResourceByPeriod(IEnumerable<EmployeeSettingToSendMail> employeeSettings);

        void SendNotifyMail(IEnumerable<EmployeeSettingToSendMail> employeeSettings);
    }
}