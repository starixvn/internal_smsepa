namespace SMS.EPA.Site.Controllers.API.JobLevelStandard
{
    using System.Collections.Generic;
    using System.Web.Http;
    using GO.UserControl.Client.Core.Enumerables;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Criteria.JobLevelStandard.KPI;
    using SMS.EPA.Model.JobLevelStandard;

    public class JobLevelStandardKPIController : BaseController
    {
        private readonly IBaseService baseService;

        public JobLevelStandardKPIController()
            : this(null)
        {
        }

        public JobLevelStandardKPIController(IBaseService baseService)
        {
            this.baseService = baseService ?? new BaseService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardKPI, Policy = Policy.Read)]
        public IEnumerable<JobLevelStandardKPI> GetKPI([FromUri] GetJobLevelStandardKPICriteria criteria)
        {
            return baseService.GetData<JobLevelStandardKPI>(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardKPI, Policy = Policy.Host)]
        public CommandResult Create(CreateJobLevelStandardKPICriteria criteria)
        {
            criteria.CreatedBy = CurrentEmployeeId;
            return baseService.ExecuteNonQuery(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardKPI, Policy = Policy.Host)]
        public CommandResult Update(UpdateJobLevelStandardKPICriteria criteria)
        {
            criteria.ModifiedBy = CurrentEmployeeId;
            return baseService.ExecuteNonQuery(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardKPI, Policy = Policy.Host)]
        public CommandResult UpdateStatus(UpdateStatusJobLevelStandardKPICriteria criteria)
        {
            criteria.ModifiedBy = CurrentEmployeeId;
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);
            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardKPI, Policy = Policy.Host)]
        public CommandResult Delete(int kpiId)
        {
            var criteria = new DeleteJobLevelStandardKPICriteria
            {
                KPIId = kpiId,
                ModifiedBy = CurrentEmployeeId
            };
            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        public byte GetUserPermission()
        {
            return GetPermissionOnFunction(EPAFunctionKey.JobLevelStandardKPI);
        }
    }
}