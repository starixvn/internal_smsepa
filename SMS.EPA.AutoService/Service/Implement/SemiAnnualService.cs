namespace SMS.EPA.AutoService.Service
{
    using System.Collections.Generic;
    using System.Linq;
    using Criteria;
    using Implement;

    public class SemiAnnualService : BasePeriodService, ISemiAnnualService
    {
        public void CheckSemiAnnualPeriod()
        {
            IEnumerable<int> semiAnnualPeriods = GetStartedPeriods(new GetSemiAnnualPeriodStartedCriteria());

            foreach (int periodId in semiAnnualPeriods)
            {
                InsertEmployeeToPeriod(periodId, new InsertEmployeeToSemiAnnualPeriodCriteria(periodId), "Auto add employee to semi annual period");
                GenerateEPA(periodId, "Auto generate EPA for semi annual period");
                SendStartEmail(periodId);
            }
        }
    }
}