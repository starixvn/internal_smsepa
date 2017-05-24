namespace SMS.EPA.AutoService.Service
{
    using Core.Model;
    using Criteria;
    using Implement;
    using Model;
    using System.Collections.Generic;
    using System.Linq;

    public class ProbationService : BasePeriodService, IProbationService
    {
        public void CheckProbationPeriod()
        {
            IEnumerable<AutoEmployeePeriodModel> employeePeriods =
                GetEmployeeEvaluation(new GetEmployeeProbationEvaluationCriteria());

            if (employeePeriods != null && employeePeriods.Any())
            {
                foreach (AutoEmployeePeriodModel employeePeriod in employeePeriods)
                {
                    if (!string.IsNullOrWhiteSpace(employeePeriod.EmployeeIds) && employeePeriod.PeriodId > 0)
                    {
                        InsertEmployeeToPeriod(employeePeriod);
                        GenerateEPA(
                            employeePeriod,
                            "Result of auto generate EPA to all employees of current probation period ");
                        SendStartEmail(employeePeriod.PeriodId, employeePeriod.EmployeeIds);
                    }
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
            var comandResult = new CommandResult();

            if (criteria.IsValid())
            {
                comandResult = Repository.Insert(criteria);
            }

            string messageResult = comandResult.IsSuccessful ? "0" : comandResult.Message;
            Log.Info("Result of auto add employee to probation period " + employeePeriod.PeriodId + ": " + messageResult);
        }
    }
}