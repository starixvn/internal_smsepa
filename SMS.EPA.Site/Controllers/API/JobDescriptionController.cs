namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Enumerations;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria.JobDescription;
    using SMS.EPA.Model.JobDescription;
    using SMS.EPA.Services;

    public class JobDescriptionController : BaseController
    {
        private readonly IBaseService baseService;
        private readonly IJobDescriptionService jobDescriptionService;

        public JobDescriptionController() : this(null, null)
        {
        }

        public JobDescriptionController(IBaseService baseService, IJobDescriptionService jobDescriptionService)
        {
            this.baseService = baseService ?? new BaseService();
            this.jobDescriptionService = jobDescriptionService ?? new JobDescriptionService();
        }

        #region Group

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Read)]
        public async Task<IEnumerable<JobDescriptionGroupInfo>> GetGroupSettingInfo(GetGroupSettingJobDescriptionCriteria criteria)
        {
            return await baseService.GetDataAsync<JobDescriptionGroupInfo>(criteria);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Read)]
        public async Task<HttpResponseMessage> GetGroupSettingById(int groupId, int jobLevelId)
        {
            var groupInfo = await jobDescriptionService.GetGroupSettingByIdAsync(groupId, jobLevelId);
            if (groupInfo == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, groupInfo);
            }

            return Request.CreateResponse(HttpStatusCode.OK, groupInfo);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Host)]
        public async Task<CommandResult> EditGroupSetting(EditGroupSettingJobDescriptionCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = SessionService.EmployeeId;
            }

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        #endregion Group

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Read)]
        public async Task<IEnumerable<JobDescriptionEmployeeInfo>> GetEmployeeSettingInfo(GetEmployeeSettingJobDescriptionCriteria criteria)
        {
            return await baseService.GetDataAsync<JobDescriptionEmployeeInfo>(criteria);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Read)]
        public async Task<HttpResponseMessage> GetEmployeeSettingById(int employeeId)
        {
            var employeeInfo = await jobDescriptionService.GetEmployeeSettingByIdAsync(employeeId);
            if (employeeInfo == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, employeeInfo);
            }

            return Request.CreateResponse(HttpStatusCode.OK, employeeInfo);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Host)]
        public async Task<CommandResult> CreateJobDescriptionGroupItem(CreateJobDescriptionGroupItemCriteria criteria)
        {
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Host)]
        public CommandResult EditJobDescriptionGroupItem(EditJobDescriptionGroupItemCriteria criteria)
        {
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Host)]
        public CommandResult DeleteJobDescriptionGroupItem(int groupItemId)
        {
            var criteria = new DeleteJobDescriptionGroupItemCriteria
            {
                GroupItemId = groupItemId
            };

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Host)]
        public async Task<CommandResult> CreateJobDescriptionItem(CreateJobDescriptionItemCriteria criteria)
        {
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Host)]
        public CommandResult EditJobDescriptionItem(EditJobDescriptionItemCriteria criteria)
        {
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Host)]
        public CommandResult DeleteJobDescriptionItem(int itemId)
        {
            var criteria = new DeleteJobDescriptionItemCriteria
            {
                ItemId = itemId
            };

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Host)]
        public CommandResult EditEmployeeSetting(EditEmployeeSettingJobDescriptionCriteria criteria)
        {

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        public byte GetUserPermission()
        {
            return SessionService.GetPermission(EPAFunctionKey.JobDescription);
        }
    }
}