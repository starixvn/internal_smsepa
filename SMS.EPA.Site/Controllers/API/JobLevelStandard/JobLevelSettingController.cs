namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Extensions;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevelStandard;
    using SMS.EPA.Model.JobLevelStandard;

    public class JobLevelSettingController : BaseController
    {
        private readonly IBaseService baseService;

        public JobLevelSettingController() : this(null)
        {
        }

        public JobLevelSettingController(IBaseService baseService)
        {
            this.baseService = baseService ?? new BaseService();
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelSetting, Policy = Policy.Read)]
        public async Task<IList<JobLevelSettingSummary>> GetSettings(GetJobLevelSettingSummaryCriteria criteria)
        {
            var settings = await baseService.GetDataAsync<JobLevelSettingSummary>(criteria);
            return settings.ToList();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelSetting, Policy = Policy.Read)]
        public async Task<IList<JobLevelSetting>> GetBySectionId(int sectionId)
        {
            var criteria = new GetJobLevelSettingBySectionIdCriteria { SectionId = sectionId };
            var settings = await baseService.GetDataAsync<JobLevelSetting>(criteria);
            return settings.ToList();
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelSetting, Policy = Policy.Host)]
        public async Task<CommandResult> Edit(EditJobLevelSettingCommand command)
        {
            var commandResult = new CommandResult();
            if (command != null)
            {
                command.SettingXml = command.Settings.ToXML().ToString();
                command.ModifiedBy = SessionService.EmployeeId;
                commandResult = await baseService.ExecuteNonQueryAsync(command);
            }

            return commandResult;
        }

        public byte GetPermission()
        {
            return SessionService.GetPermission(EPAFunctionKey.JobLevelSetting);
        }
    }
}