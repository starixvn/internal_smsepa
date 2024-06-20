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
        public IList<JobLevelSettingSummary> GetSettings(GetJobLevelSettingSummaryCriteria criteria)
        {
            var settings = baseService.GetData<JobLevelSettingSummary>(criteria);
            return settings.ToList();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelSetting, Policy = Policy.Read)]
        public IList<JobLevelSetting> GetBySectionId(int sectionId)
        {
            var criteria = new GetJobLevelSettingBySectionIdCriteria { SectionId = sectionId };
            var settings = baseService.GetData<JobLevelSetting>(criteria);
            return settings.ToList();
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelSetting, Policy = Policy.Host)]
        public CommandResult Edit(EditJobLevelSettingCommand command)
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

        public byte GetPermission()
        {
            return GetPermissionOnFunction(EPAFunctionKey.JobLevelSetting);
        }
    }
}