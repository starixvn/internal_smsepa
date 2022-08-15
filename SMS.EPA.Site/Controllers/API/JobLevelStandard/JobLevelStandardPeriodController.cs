namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using SMS.EPA.Core.Extensions;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevelStandard;
    using SMS.EPA.Model.JobLevelStandard.Period;

    public class JobLevelStandardPeriodController : BaseController
    {
        private readonly IBaseService baseService;

        public JobLevelStandardPeriodController() : this(null)
        {
        }

        public JobLevelStandardPeriodController(IBaseService baseService)
        {
            this.baseService = baseService ?? new BaseService();
        }

        [HttpPost]
        public IList<JobLevelStandardPeriod> GetPeriods(GetJobLevelStandardPeriodCriteria criteria)
        {
            var tuple = baseService.FetchMultiple<JobLevelStandardPeriod, JobLevelStandardPeriodSetting>(criteria);
            IList<JobLevelStandardPeriod> periods = tuple.Item1.ToList();
            foreach(var period in periods)
            {
                period.Settings = tuple.Item2.Where(x => x.PeriodId == period.PeriodId).ToList();
            }

            return periods;
        }

        public JobLevelStandardPeriod GetById(int periodId)
        {
            var criteria = new GetJobLevelStandardPeriodByIdCriteria { PeriodId = periodId };
            var tuple = baseService.FetchMultiple<JobLevelStandardPeriod, JobLevelStandardPeriodSetting>(criteria);
            JobLevelStandardPeriod period = tuple.Item1.First();
            period.Settings = tuple.Item2.ToList();

            return period;
        }

        [HttpPost]
        public CommandResult Create(CreateJobLevelStandardPeriodCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.SettingXml = command.Settings.ToXML().ToString();
                command.IsActive = true;
                command.CreatedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        [HttpPut]
        public CommandResult Edit(EditJobLevelStandardPeriodCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.SettingXml = command.Settings.ToXML().ToString();
                command.ModifiedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        [HttpPut]
        public CommandResult EditStatus(EditStatusJobLevelStandardPeriodCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = SessionService.EmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        [HttpDelete]
        public CommandResult Delete(int periodId)
        {
            var criteria = new DeleteJobLevelStandardPeriodCommand
            {
                PeriodId = periodId,
                ModifiedBy = SessionService.EmployeeId
            };
            return baseService.ExecuteNonQuery(criteria);
        }
    }
}