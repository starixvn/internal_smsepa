namespace SMS.EPA.AutoService.Service
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Implement;
    using Model;

    public class ProbationService : BasePeriodService, IProbationService
    {
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(ProbationService));

        public void CheckProbationPeriod()
        {
            IEnumerable<AutoEmployeePeriodModel> employeePeriods = GetEmployeeEvaluation(new GetEmployeeProbationEvaluationCriteria());

            foreach (AutoEmployeePeriodModel employeePeriod in employeePeriods)
            {
                if (!string.IsNullOrWhiteSpace(employeePeriod.EmployeeIds) && employeePeriod.PeriodId > 0)
                {
                    InsertEmployeeToPeriod(employeePeriod);
                    GenerateEPA(employeePeriod, "Auto generate EPA for probation period");
                    SendReminderStartEmail(employeePeriod.PeriodId, employeePeriod.EmployeeIds);
                }
            }
        }

        private void InsertEmployeeToPeriod(AutoEmployeePeriodModel employeePeriod)
        {
            var criteria = new InsertEmployeeToProbationPeriodCriteria
            {
                PeriodId = employeePeriod.PeriodId,
                EmployeeIds = employeePeriod.EmployeeIds
            };
            CommandResult commandResult = BaseService.ExecuteNonQuery(criteria);

            if (!commandResult.IsSuccessful)
            {
                logger.Error("Auto add employee to probation period " + employeePeriod.PeriodId + ": " + commandResult.Message);
            }
        }
    }
}