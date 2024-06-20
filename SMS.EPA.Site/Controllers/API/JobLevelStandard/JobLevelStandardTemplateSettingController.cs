namespace SMS.EPA.Site.Controllers.API.JobLevelStandard
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
    using GO.UserControl.Client.Core.Enumerables;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobLevelStandard.TemplateSetting;
    using SMS.EPA.Model.JobLevelStandard.TemplateSetting;

    public class JobLevelStandardTemplateSettingController : BaseController
    {
        private readonly IBaseService baseService;

        public JobLevelStandardTemplateSettingController()
            : this(null)
        {
        }

        public JobLevelStandardTemplateSettingController(IBaseService baseService)
        {
            this.baseService = baseService ?? new BaseService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardTemplateSetting, Policy = Policy.Read)]
        public IEnumerable<JobLevelStandardTemplateSetting> GetTemplateSetting([FromUri] GetJobLevelStandardTemplateSettingCriteria criteria)
        {
            return baseService.GetData<JobLevelStandardTemplateSetting>(criteria).ToList();
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardTemplateSetting, Policy = Policy.Host)]
        public CommandResult Delete(int objectId, byte objectTypeId)
        {
            var criteria = new DeleteJobLevelStandardTemplateSettingCriteria
            {
                ObjectId = objectId,
                ObjectTypeId = objectTypeId,
                ModifiedBy = CurrentEmployeeId
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardTemplateSetting, Policy = Policy.Host)]
        public CommandResult Assign(AssignJobLevelStandardTemplateSettingCriteria criteria)
        {
            criteria.ModifiedBy = CurrentEmployeeId;
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        public byte GetUserPermission()
        {
            return GetPermissionOnFunction(EPAFunctionKey.JobLevelStandardTemplateSetting);
        }
    }
}