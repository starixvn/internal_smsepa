namespace SMS.EPA.AutoService.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Constants;
    using Core.Enumerations;
    using Core.Model;
    using Criteria;
    using EPA.Criteria;
    using Implement;
    using Model;

    public class MonthlyService : BasePeriodService, IMonthlyService
    {
        public void CheckMonthlyPeriod()
        {
            IEnumerable<int> monthlyPeriods = GetStartedPeriods(new GetMonthlyPeriodStartedCriteria());

            foreach (int periodId in monthlyPeriods)
            {
                InsertEmployeeToPeriod(periodId, new InsertEmployeeToMonthlyPeriodCriteria(periodId), "Auto add employee to monthly period");
                GenerateEPA(periodId, "Auto generate EPA for monthly period");
                SendStartEmail(periodId);
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

                    if (!commandResult.IsSuccessful)
                    {
                        Log.Info($"Error create monthly period: {commandResult.Message}");
                    }
                }
            }
        }
    }
}