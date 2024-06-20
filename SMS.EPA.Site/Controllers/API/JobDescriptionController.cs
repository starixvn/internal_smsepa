namespace SMS.EPA.Site.Controllers.API
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using GO.UserControl.Client.Core.Enumerables;
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
        public IEnumerable<JobDescriptionGroupInfo> GetGroupSettingInfo(GetGroupSettingJobDescriptionCriteria criteria)
        {
            return baseService.GetData<JobDescriptionGroupInfo>(criteria);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Read)]
        public IEnumerable<JobDescriptionGroupInfo> GetGroupHasSetting()
        {
            var criteria = new GetGroupSettingJobDescriptionCriteria();
            var groups = baseService.GetData<JobDescriptionGroupInfo>(criteria);
            return groups.Where(x => !string.IsNullOrWhiteSpace(x.JobLevels));
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Read)]
        public HttpResponseMessage GetGroupSettingById(int groupId, int jobLevelId)
        {
            var groupInfo = jobDescriptionService.GetGroupSettingById(groupId, jobLevelId);
            if (groupInfo == null)
            {
                return Request.CreateResponse(HttpStatusCode.NotFound, groupInfo);
            }

            return Request.CreateResponse(HttpStatusCode.OK, groupInfo);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Host)]
        public CommandResult EditGroupSetting(EditGroupSettingJobDescriptionCriteria criteria)
        {
            criteria.ModifiedBy = CurrentEmployeeId;
            return baseService.ExecuteNonQuery(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Host)]
        public CommandResult CopyGroupSetting(CopyGroupSettingJobDescriptionCriteria criteria)
        {
            criteria.ActorId = CurrentEmployeeId;
            return baseService.ExecuteNonQuery(criteria);
        }

        #endregion Group

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Read)]
        public IEnumerable<JobDescriptionEmployeeInfo> GetEmployeeSettingInfo(GetEmployeeSettingJobDescriptionCriteria criteria)
        {
            return baseService.GetData<JobDescriptionEmployeeInfo>(criteria);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Read)]
        public IEnumerable<JobDescriptionEmployeeInfo> GetEmployeeHasSetting()
        {
            var criteria = new GetEmployeeSettingJobDescriptionCriteria();
            return baseService.GetData<JobDescriptionEmployeeInfo>(criteria);
        }

        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Read)]
        public HttpResponseMessage GetEmployeeSettingById(int employeeId)
        {
            var employeeInfo = jobDescriptionService.GetEmployeeSettingById(employeeId);
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
        public CommandResult CopyEmployeeSetting(CopyEmployeeSettingJobDescriptionCriteria criteria)
        {
            criteria.ActorId = CurrentEmployeeId;
            return baseService.ExecuteNonQuery(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Host)]
        public CommandResult CreateJobDescriptionGroupItem(CreateJobDescriptionGroupItemCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
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
        public CommandResult CreateJobDescriptionItem(CreateJobDescriptionItemCriteria criteria)
        {
            return baseService.ExecuteNonQuery(criteria);
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