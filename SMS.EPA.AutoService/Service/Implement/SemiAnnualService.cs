namespace SMS.EPA.AutoService.Service
{
    using SMS.EPA.AutoService.Criteria;

    public class SemiAnnualService : BasePeriodService, ISemiAnnualService
    {
        public void CheckSemiAnnualPeriod()
        {
            var semiAnnualPeriods = BaseService.GetData<int>(new GetSemiAnnualPeriodStartedCriteria());
            foreach (int periodId in semiAnnualPeriods)
            {
                InsertEmployeeToPeriod(periodId, new InsertEmployeeToSemiAnnualPeriodCriteria(periodId), "Auto add employee to semi annual period");
                GenerateEPA(periodId, "Auto generate EPA for semi annual period");
                SendReminderStartEmail(periodId);
            }
        }
    }
}