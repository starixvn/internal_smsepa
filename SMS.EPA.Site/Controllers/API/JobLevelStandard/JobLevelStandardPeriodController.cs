namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using HRM.UserControl.Client.Core.Enumerables;
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
        public async Task<IList<JobLevelStandardPeriod>> GetPeriods(GetJobLevelStandardPeriodCriteria criteria)
        {
            var tuple = await baseService.FetchMultipleAsync<JobLevelStandardPeriod, JobLevelStandardPeriodSetting>(criteria);
            IList<JobLevelStandardPeriod> periods = tuple.Item1.ToList();
            foreach (var period in periods)
            {
                period.Settings = tuple.Item2.Where(x => x.PeriodId == period.PeriodId).ToList();
            }

            return periods;
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Read)]
        public async Task<JobLevelStandardPeriod> GetById(int periodId)
        {
            var criteria = new GetJobLevelStandardPeriodByIdCriteria { PeriodId = periodId };
            var tuple = await baseService.FetchMultipleAsync<JobLevelStandardPeriod, JobLevelStandardPeriodSetting>(criteria);
            JobLevelStandardPeriod period = tuple.Item1.First();
            period.Settings = tuple.Item2.ToList();

            return period;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public async Task<CommandResult> Create(CreateJobLevelStandardPeriodCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.SettingXml = command.Settings.ToXML().ToString();
                command.IsActive = true;
                command.CreatedBy = CurrentEmployeeId;
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public async Task<CommandResult> Edit(EditJobLevelStandardPeriodCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.SettingXml = command.Settings.ToXML().ToString();
                command.ModifiedBy = CurrentEmployeeId;
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public async Task<CommandResult> EditStatus(EditStatusJobLevelStandardPeriodCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = CurrentEmployeeId;
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public async Task<CommandResult> Delete(int periodId)
        {
            var criteria = new DeleteJobLevelStandardPeriodCommand
            {
                PeriodId = periodId,
                ModifiedBy = CurrentEmployeeId
            };
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Read)]
        public IList<PeriodEmployeeSetting> GetEmployeeSettings(GetPeriodEmployeeSettingCriteria criteria)
        {
            return jobLevelStandardPeriodService.GetEmployeeSettings(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public async Task<CommandResult> AddEmployeeSetting(AddPeriodEmployeeSettingCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = CurrentEmployeeId;
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public async Task<CommandResult> UpdateJobLevel(UpdateJobLevelEmployeeSettingCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = CurrentEmployeeId;
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public async Task<CommandResult> EditEmployeeSetting(IList<PeriodEmployeeSetting> employeeSettings)
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
                PeriodId = employeeSettings.First().PeriodId,
                TemplateId = employeeSettings.First().TemplateId,
                SettingXml = settings.ToXML().ToString(),
                ModifiedBy = CurrentEmployeeId
            };
            CommandResult commandResult = await baseService.ExecuteNonQueryAsync(command);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public async Task<CommandResult> EditStatusEmployeeSetting(EditStatusPeriodEmployeeSettingCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = CurrentEmployeeId;
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Read)]
        public async Task<IList<JobLevelSettingSummary>> GetJobLevelSettings(GetPeriodJobLevelSettingSummaryCriteria criteria)
        {
            var settings = await baseService.GetDataAsync<JobLevelSettingSummary>(criteria);
            return settings.ToList();
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Read)]
        public async Task<IList<JobLevelSetting>> GetJobLevelSettingBySectionId(GetPeriodJobLevelSettingBySectionIdCriteria criteria)
        {
            var settings = await baseService.GetDataAsync<JobLevelSetting>(criteria);
            return settings.ToList();
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public async Task<CommandResult> EditJobLevelSetting(EditPeriodJobLevelSettingCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.SettingXml = command.Settings.ToXML().ToString();
                command.ModifiedBy = CurrentEmployeeId;
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardPeriod, Policy = Policy.Host)]
        public async Task<CommandResult> CopyDefaultJobLevelSetting(CopyDefaultJobLevelSettingCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.ModifiedBy = CurrentEmployeeId;
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        public byte GetPermission()
        {
            return GetPermissionOnFunction(EPAFunctionKey.JobLevelStandardPeriod);
        }
    }
}