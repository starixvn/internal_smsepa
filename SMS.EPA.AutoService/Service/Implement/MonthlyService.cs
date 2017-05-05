namespace SMS.EPA.AutoService.Service
{
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Criteria;
    using EPA.Criteria;
    using Implement;
    using Model;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class MonthlyService : PeriodicService, IMonthlyService
    {
        public void CheckMonthlyPeriod()
        {
            IEnumerable<int> monthlyPeriods = GetStartedPeriods(new GetMonthlyPeriodStartedCriteria());
            Log.Info($"Result of check the start of monthly period: {string.Join(",", monthlyPeriods)}");

            if (monthlyPeriods.Any())
            {
                foreach (int periodId in monthlyPeriods)
                {
                    InsertEmployeeToPeriod(
                        periodId,
                        new InsertEmployeeToMonthlyPeriodCriteria(periodId),
                        "Result of auto add employee to monthly period");
                    GenerateEPA(periodId, "Result of auto generate EPA to all employees of current monthly period");
                    SendStartEmail(periodId);
                }
            }
        }

        public void CreateMonthlyPeriod()
        {
            IEnumerable<FlowModel> processSettings = PeriodService.GetEvaluationGlobalSettingByTargetTypeId((int)TargetType.Monthly);

            if (processSettings != null && processSettings.Any())
            {
                DateTime today = DateTime.Today;
                int startDayCreateMonthlyPeriod = AppSetting.StartDayCreateMonthlyPeriod;

                if (today.Day.Equals(startDayCreateMonthlyPeriod))
                {
                    var criteria = new AutoCreateMonthlyPeriodCriteria
                    {
                        Name = today.AddMonths(1).ToString(Constants.FormatMonthlyPeriodName),
                        StartDate = today,
                        EndDate = today.AddDays(processSettings.Sum(p => p.Duration))
                    };

                    CommandResult commandResult = PeriodService.AutoCreateMonthlyPeriod(criteria);
                    Log.Info(
                        $"Result of Create monthly period: {(commandResult.IsSuccessful ? "0" : commandResult.Message)}");
                }
            }
        }
    }
}