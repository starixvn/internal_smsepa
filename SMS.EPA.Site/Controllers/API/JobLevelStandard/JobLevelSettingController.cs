namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
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
        public IList<JobLevelSettingSummary> GetSettings(GetJobLevelSettingSummaryCriteria criteria)
        {
            return baseService.GetData<JobLevelSettingSummary>(criteria).ToList();
        }

        [HttpGet]
        public IList<JobLevelSetting> GetBySectionId(int sectionId)
        {
            var criteria = new GetJobLevelSettingBySectionIdCriteria { SectionId = sectionId };
            return baseService.GetData<JobLevelSetting>(criteria).ToList();
        }

        [HttpPut]
        public CommandResult Edit(EditJobLevelSettingCommand command)
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
    }
}