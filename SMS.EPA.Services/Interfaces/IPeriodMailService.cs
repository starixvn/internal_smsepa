namespace SMS.EPA.Services.Interfaces
{
    using System.Collections.Generic;
    using Model;

    public interface IPeriodMailService
    {
        void SendReminderMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings, int mailMode);

        void SendEndEvaluationMailToSelf(EPAModel evaluationInfo, float? averagePerformanceScore, float disciplineScore);

        void SendEndEvaluationMailToHumanResourceByEmployee(EPAModel evaluationInfo);

        void SendEndEvaluationMailToHumanResourceByPeriod(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendNotifyMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);
    }
}