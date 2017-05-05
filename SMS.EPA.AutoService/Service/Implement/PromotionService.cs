namespace SMS.EPA.AutoService.Service
{
    using Criteria;
    using Implement;
    using Model;
    using System.Collections.Generic;
    using System.Linq;

    public class PromotionService : StaticPeriodService, IPromotionService
    {
        public void CheckPromotionPeriod()
        {
            IEnumerable<AutoEmployeePeriodModel> employeePeriods =
                GetEmployeeEvaluation(new GetEmployeePromotionEvaluationCriteria());

            if (employeePeriods != null && employeePeriods.Any())
            {
                foreach (AutoEmployeePeriodModel employeePeriod in employeePeriods)
                {
                    if (!string.IsNullOrWhiteSpace(employeePeriod.EmployeeIds) && employeePeriod.PeriodId > 0)
                    {
                        GenerateEPA(
                            employeePeriod,
                            "Result of auto generate EPA to all employees of current promotion period ");
                        SendStartEmail(employeePeriod.PeriodId, employeePeriod.EmployeeIds);
                    }
                }
            }
        }
    }
}