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

    ////[ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Read)]
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

        [HttpPost]
        public async Task<IEnumerable<JobDescriptionEmployeeInfo>> GetEmployeeSettingInfo(GetEmployeeSettingJobDescriptionCriteria criteria)
        {
            return await baseService.GetDataAsync<JobDescriptionEmployeeInfo>(criteria);
        }

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
        ////[ApiAuthorize(FunctionKey = EPAFunctionKey.JobDescription, Policy = Policy.Host)]
        public async Task<CommandResult> CreateJobDescriptionGroupItem(CreateJobDescriptionGroupItemCriteria criteria)
        {
            return await baseService.ExecuteNonQueryAsync(criteria);
        }

        public byte GetUserPermission()
        {
            return SessionService.GetPermission(EPAFunctionKey.JobDescription);
        }
    }
}