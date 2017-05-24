namespace SMS.EPA.AutoService.Service
{
    using Criteria;
    using Implement;
    using System.Collections.Generic;
    using System.Linq;

    public class SemiAnnualService : BasePeriodService, ISemiAnnualService
    {
        public void CheckSemiAnnualPeriod()
        {
            IEnumerable<int> semiAnnualPeriods = GetStartedPeriods(new GetSemiAnnualPeriodStartedCriteria());
            Log.Info($"Result of check the start of semi annual period: {string.Join(",", semiAnnualPeriods)}");

            if (semiAnnualPeriods.Any())
            {
                foreach (int periodId in semiAnnualPeriods)
                {
                    InsertEmployeeToPeriod(
                        periodId,
                        new InsertEmployeeToSemiAnnualPeriodCriteria(periodId),
                        "Result of auto add employee to semi annual period");
                    GenerateEPA(periodId, "Result of auto generate EPA to all employees of current semi annual period");
                    SendStartEmail(periodId);
                }
            }
        }
    }
}