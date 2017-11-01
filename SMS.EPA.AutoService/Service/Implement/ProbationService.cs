namespace SMS.EPA.AutoService.Service
{
    using System.Collections.Generic;
    using Core.Model;
    using Criteria;
    using Fanex.Logging;
    using Implement;
    using Model;

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
                    SendStartEmail(employeePeriod.PeriodId, employeePeriod.EmployeeIds);
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
            var commandResult = new CommandResult();

            if (criteria.IsValid())
            {
                commandResult = Repository.ExecuteNonQuery(criteria);
            }

            if (!commandResult.IsSuccessful)
            {
                LogService.Error("Auto add employee to probation period " + employeePeriod.PeriodId + ": " + commandResult.Message);
            }
        }
    }
}