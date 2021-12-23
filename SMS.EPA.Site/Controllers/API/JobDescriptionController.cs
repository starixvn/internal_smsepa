namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web.Http;
    using HRM.UserControl.Client.Core.Enumerables;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
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
        public async Task<IEnumerable<JobDescriptionGroupInfo>> GetGroupHasSetting()
        {
            var criteria = new GetGroupSettingJobDescriptionCriteria();
            var groups = await baseService.GetDataAsync<JobDescriptionGroupInfo>(criteria);
            return groups.Where(x => !string.IsNullOrWhiteSpace(x.JobLevels));
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
                criteria.ModifiedBy = CurrentEmployeeId;
            }

            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Host)]
        public async Task<CommandResult> CopyGroupSetting(CopyGroupSettingJobDescriptionCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ActorId = CurrentEmployeeId;
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
        public async Task<IEnumerable<JobDescriptionEmployeeInfo>> GetEmployeeHasSetting()
        {
            var criteria = new GetEmployeeSettingJobDescriptionCriteria();
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
        public CommandResult EditEmployeeSetting(EditEmployeeSettingJobDescriptionCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Host)]
        public async Task<CommandResult> CopyEmployeeSetting(CopyEmployeeSettingJobDescriptionCriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ActorId = CurrentEmployeeId;
            }

            return await baseService.ExecuteNonQueryAsync(criteria);
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
            return baseService.ExecuteNonQuery(criteria);
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Host)]
        public CommandResult DeleteJobDescriptionGroupItem(int groupItemId)
        {
            var criteria = new DeleteJobDescriptionGroupItemCriteria
            {
                GroupItemId = groupItemId
            };
            return baseService.ExecuteNonQuery(criteria);
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
            return baseService.ExecuteNonQuery(criteria);
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Host)]
        public CommandResult DeleteJobDescriptionItem(int itemId)
        {
            var criteria = new DeleteJobDescriptionItemCriteria
            {
                ItemId = itemId
            };
            return baseService.ExecuteNonQuery(criteria);
        }

        public byte GetUserPermission()
        {
            return GetPermissionOnFunction(EPAFunctionKey.JobDescription);
        }
    }
}