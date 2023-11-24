namespace SMS.EPA.AutoService.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Fanex.Logging;
    using SMS.EPA.AutoService.Criteria;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;

    public class MonthlyService : BasePeriodService, IMonthlyService
    {
        public void CheckMonthlyPeriod()
        {
            var monthlyPeriods = BaseService.GetData<int>(new GetMonthlyPeriodStartedCriteria());
            foreach (int periodId in monthlyPeriods)
            {
                InsertEmployeeToPeriod(periodId, new InsertEmployeeToMonthlyPeriodCriteria(periodId), "Auto add employee to monthly period");
                GenerateEPA(periodId, "Auto generate EPA for monthly period");
                SendReminderStartEmail(periodId);
            }
        }

        public void CreateMonthlyPeriod()
        {
            IEnumerable<FlowModel> processSettings = GetEvaluationGlobalSetting();

            if (processSettings != null && processSettings.Any())
            {
                var today = DateTimeOffset.Now.Date;
                int startDayCreateMonthlyPeriod = AppSetting.StartDayCreateMonthlyPeriod;

                if (today.Day.Equals(startDayCreateMonthlyPeriod))
                {
                    var criteria = new AutoCreateMonthlyPeriodCriteria
                    {
                        Name = today.AddMonths(1).ToString(Constants.FormatMonthlyPeriodName),
                        StartDate = today,
                        EndDate = today.AddDays(processSettings.Sum(p => p.Duration))
                    };
                    CommandResult commandResult = BaseService.ExecuteNonQuery(criteria);

                    if (!commandResult.IsSuccessful)
                    {
                        LogService.Error(string.Format("Error create monthly period: {0}", commandResult.Message));
                    }
                }
            }
        }

        private IEnumerable<FlowModel> GetEvaluationGlobalSetting()
        {
            var criteria = new GetEvaluationGlobalSettingByTargetTypeIdCriteria
            {
                TargetTypeId = (byte)TargetType.Monthly
            };
            IEnumerable<FlowModel> processSettings = BaseService.GetData<FlowModel>(criteria);

            return processSettings;
        }
    }
}