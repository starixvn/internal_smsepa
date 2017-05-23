namespace SMS.EPA.AutoService.Service
{
    using Model;
    using System.Collections.Generic;

    public interface IAutoMailService
    {
        void SendReminderMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings, int mailMode);

        void SendEndEvaluationMailToSelf(EPAModel evaluationInfo, float? averagePerformanceScore, float disciplineScore);

        void SendEndEvaluationMailToHumanResourceByEmployee(EPAModel evaluationInfo);

        void SendEndEvaluationMailToHumanResourceByPeriod(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);
    }
}