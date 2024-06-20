namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using GO.UserControl.Client.Core.Enumerables;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Extensions;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevelStandard;
    using SMS.EPA.Criteria.JobLevelStandard.Period;
    using SMS.EPA.Model.JobLevelStandard;
    using SMS.EPA.Model.JobLevelStandard.Period;
    using SMS.EPA.Services;

    public class JobLevelStandardPeriodController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IJobLevelStandardPeriodService jobLevelStandardPeriodService;

        public JobLevelStandardPeriodController() : this(null, null)
        {
        }

        public JobLevelStandardPeriodController(
            IBaseService baseService,
            IJobLevelStandardPeriodService jobLevelStandardPeriodService)
        {
            this.baseService = baseService ?? new BaseService();
            this.jobLevelStandardPeriodService = jobLevelStandardPeriodService ?? new JobLevelStandardPeriodService();
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Read)]
        public IList<JobLevelStandardPeriod> GetPeriods(GetJobLevelStandardPeriodCriteria criteria)
        {
            (var periods, var settings) = baseService.FetchMultiple<JobLevelStandardPeriod, JobLevelStandardPeriodSetting>(criteria);
            foreach (var period in periods)
            {
                period.Settings = settings.Where(x => x.PeriodId == period.PeriodId).ToList();
            }

            return periods.ToList();
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Read)]
        public JobLevelStandardPeriod GetById(int periodId)
        {
            var criteria = new GetJobLevelStandardPeriodByIdCriteria { PeriodId = periodId };
            (var periods, var settings) = baseService.FetchMultiple<JobLevelStandardPeriod, JobLevelStandardPeriodSetting>(criteria);
            var period = periods.First();
            period.Settings = settings.ToList();

            return period;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult Create(CreateJobLevelStandardPeriodCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.SettingXml = command.Settings.ToXML().ToString();
                command.IsActive = true;
                command.CreatedBy = CurrentEmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult Edit(EditJobLevelStandardPeriodCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.SettingXml = command.Settings.ToXML().ToString();
                command.ModifiedBy = CurrentEmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult EditStatus(EditStatusJobLevelStandardPeriodCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = CurrentEmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult Delete(int periodId)
        {
            var criteria = new DeleteJobLevelStandardPeriodCommand
            {
                PeriodId = periodId,
                ModifiedBy = CurrentEmployeeId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Read)]
        public IList<PeriodEmployeeSetting> GetEmployeeSettings(GetPeriodEmployeeSettingCriteria criteria)
        {
            return jobLevelStandardPeriodService.GetEmployeeSettings(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult AddEmployeeSetting(AddPeriodEmployeeSettingCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = CurrentEmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult UpdateJobLevel(UpdateJobLevelEmployeeSettingCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = CurrentEmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult EditEmployeeSetting(IList<PeriodEmployeeSetting> employeeSettings)
        {
            var settings = new List<PeriodProcessSetting>();
            foreach (var setting in employeeSettings)
            {
                settings.Add(setting.Employee);
                settings.AddRange(setting.Evaluators);
                settings.AddRange(setting.Managements);
            }

            var command = new EditPeriodEmployeeSettingCommand
            {
                PeriodId = employeeSettings[0].PeriodId,
                TemplateId = employeeSettings[0].TemplateId,
                SettingXml = settings.ToXML().ToString(),
                ModifiedBy = CurrentEmployeeId
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(command);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult EditStatusEmployeeSetting(EditStatusPeriodEmployeeSettingCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = CurrentEmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Read)]
        public IList<JobLevelSettingSummary> GetJobLevelSettings(GetPeriodJobLevelSettingSummaryCriteria criteria)
        {
            var settings = baseService.GetData<JobLevelSettingSummary>(criteria);
            return settings.ToList();
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Read)]
        public IList<JobLevelSetting> GetJobLevelSettingBySectionId(GetPeriodJobLevelSettingBySectionIdCriteria criteria)
        {
            var settings = baseService.GetData<JobLevelSetting>(criteria);
            return settings.ToList();
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult EditJobLevelSetting(EditPeriodJobLevelSettingCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.SettingXml = command.Settings.ToXML().ToString();
                command.ModifiedBy = CurrentEmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public CommandResult CopyDefaultJobLevelSetting(CopyDefaultJobLevelSettingCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = CurrentEmployeeId;
                commandResult = baseService.ExecuteNonQuery(command);
            }

            return commandResult;
        }

        public byte GetPermission()
        {
            return GetPermissionOnFunction(EPAFunctionKey.JobLevelStandardPeriod);
        }
    }
}