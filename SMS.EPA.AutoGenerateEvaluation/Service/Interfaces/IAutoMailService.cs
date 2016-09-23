namespace SMS.EPA.AutoGenerateEvaluation.Service
{
    using System.Collections.Generic;
    using SMS.EPA.Model;
    using SMS.EPA.Model.Period;

    public interface IAutoMailService
    {
        void SendStartMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendReminderMail(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);

        void SendEndEvaluationMailToSelf(EPAModel evaluationInfo, float averagePerformanceScore, float disciplineScore);

        void SendEndEvaluationMailToHumanResourceByEmployee(EPAModel evaluationInfo);

        void SendEndEvaluationMailToHumanResourceByPeriod(PeriodModel period, IEnumerable<PeriodFlowModel> employeeSettings);
    }
}