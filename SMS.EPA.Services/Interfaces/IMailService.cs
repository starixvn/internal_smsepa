namespace SMS.EPA.Services.Interfaces
{
    using Model;

    public interface IMailService
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Hr", Justification = "Reviewed")]
        void SendMailToHrStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToSelfStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToEvaluatorStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToReviewerStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToManagementStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailReminder(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName, int mailMode);

        void SendMailToNotifyChangeSetting(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToSelfEndEvaluation(string templateEmailPath, EPAModel evaluationInfo, float? averagePerformanceScore, float disciplineScore);

        void SendMailEndToHumanResourcesByEmployee(string templateEmailPath, EPAModel evaluationInfo, EPAProcessModel humanResource);

        void SendMailEndToHumanResourceByPeriod(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);
    }
}