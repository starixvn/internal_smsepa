namespace SMS.EPA.Site.Controllers.API
{
    using System.Web.Http;
    using GO.UserControl.Client.Core.Enumerables;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevel;

    public class JobLevelController : BaseController
    {
        private readonly IBaseService baseService;

        public JobLevelController() : this(null) { }

        public JobLevelController(IBaseService baseService)
        {
            this.baseService = baseService ?? new BaseService();
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevel, Policy = Policy.Host)]
        public CommandResult Create(CreateJobLevelCriteria criteria)
        {
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevel, Policy = Policy.Host)]
        public CommandResult Edit(EditJobLevelCriteria criteria)
        {
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevel, Policy = Policy.Host)]
        public CommandResult Delete(int jobLevelId)
        {
            var criteria = new DeleteJobLevelCriteria
            {
                JobLevelId = jobLevelId
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        public byte GetUserPermission()
        {
            return GetPermissionOnFunction(EPAFunctionKey.JobLevel);
        }
    }
}