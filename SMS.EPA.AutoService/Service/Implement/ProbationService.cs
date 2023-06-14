namespace SMS.EPA.AutoService.Service
{
    using System.Collections.Generic;
    using Fanex.Logging;
    using SMS.EPA.AutoService.Criteria;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Model;

    public class ProbationService : BasePeriodService, IProbationService
    {
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
                LogService.Error("Auto add employee to probation period " + employeePeriod.PeriodId + ": " + commandResult.Message);
            }
        }
    }
}