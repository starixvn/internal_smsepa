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
        private readonly log4net.ILog logger = log4net.LogManager.GetLogger(typeof(MonthlyService));

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
                var today = DateTime.Today;
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
                        logger.Error(string.Format("Error create monthly period: {0}", commandResult.Message));
                    }
                }
            }
        }

        private IEnumerable<FlowModel> GetEvaluationGlobalSetting()
        {
            var criteria = new GetEvaluationGlobalSettingByTargetTypeIdCriteria
            {
                TargetTypeId = (int)TargetType.Monthly
            };
            IEnumerable<FlowModel> processSettings = BaseService.GetData<FlowModel>(criteria);

            return processSettings;
        }
    }
}