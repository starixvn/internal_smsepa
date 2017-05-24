namespace SMS.EPA.Services.Interfaces
{
    using Model;

    public interface IMailService
    {
        void SendMailReminder(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName, int mailMode);

        void SendMailToNotifyChangeSetting(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToSelfEndEvaluation(string templateEmailPath, EPAModel evaluationInfo, float? averagePerformanceScore, float disciplineScore);

        void SendMailEndToHumanResourcesByEmployee(string templateEmailPath, EPAModel evaluationInfo, EPAProcessModel humanResource);

        void SendMailEndToHumanResourceByPeriod(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);
    }
}