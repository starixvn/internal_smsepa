namespace SMS.EPA.AutoService.Service
{
    using Model;
    using System.Collections.Generic;

    public interface IAutoMailService
    {
        void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendReminderMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendEndEvaluationMailToSelf(EPAModel evaluationInfo, float? averagePerformanceScore, float disciplineScore);

        void SendEndEvaluationMailToHumanResourceByEmployee(EPAModel evaluationInfo);

        void SendEndEvaluationMailToHumanResourceByPeriod(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);
    }
}