namespace SMS.EPA.Services.Interfaces
{
    using SMS.EPA.Model;

    public interface IMailService
    {
        void SendMailToHRStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToSelfStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToEvaluatorStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToReviewerStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToManagementStartDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToSelfRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToEvaluatorRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToReviewerRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToManagementRemindDate(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToNotifyChangeSetting(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);

        void SendMailToSelfEndEvaluation(string templateEmailPath, EPAModel evaluationInfo, float? averagePerformanceScore, float disciplineScore);

        void SendMailEndToHumanResourcesByEmployee(string templateEmailPath, EPAModel evaluationInfo, EPAProcessModel humanResource);

        void SendMailEndToHumanResourceByPeriod(string templateEmailPath, PeriodFlowModel employeeSetting, string periodName);
    }
}