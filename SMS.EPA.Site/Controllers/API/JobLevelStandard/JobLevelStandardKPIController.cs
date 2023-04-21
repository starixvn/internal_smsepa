using HRM.UserControl.Client.Core.Enumerables;
using SMS.EPA.Core.Attribute;
using SMS.EPA.Core.Constants;
using SMS.EPA.Core.Model;
using SMS.EPA.Core.Services;
using SMS.EPA.Criteria;
using SMS.EPA.Criteria.JobLevelStandard.KPI;
using SMS.EPA.Model.JobLevelStandard;
using System.Collections.Generic;
using System.Web.Http;

namespace SMS.EPA.Site.Controllers.API.JobLevelStandard
{
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
        public IEnumerable<JobLevelStandardKPI> GetJobLevelStandardKPI([FromUri] GetJobLevelStandardKPICriteria criteria)
        {
            return baseService.GetData<JobLevelStandardKPI>(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardKPI, Policy = Policy.Host)]
        public CommandResult UpdateStatusJobLevelStandardKPI(UpdateStatusJobLevelStandardKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentEmployeeId;
            }

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardKPI, Policy = Policy.Host)]
        public CommandResult DeleteStatusJobLevelStandardKPI(int kpiId)
        {
            var criteria = new DeleteJobLevelStandardKPICriteria()
            {
                KPIId = kpiId,
                ModifiedBy = CurrentEmployeeId
            };

            CommandResult commandResult = baseService.ExecuteNonQuery(criteria);

            return commandResult;
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardKPI, Policy = Policy.Host)]
        public CommandResult CreateJobLevelStandardKPI(CreateJobLevelStandardKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.CreatedBy = CurrentEmployeeId;
            }

            return baseService.ExecuteNonQuery(criteria);
        }

        [HttpPut]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.JobLevelStandardKPI, Policy = Policy.Host)]
        public CommandResult UpdateJobLevelStandardKPI(UpdateJobLevelStandardKPICriteria criteria)
        {
            if (criteria != null)
            {
                criteria.ModifiedBy = CurrentEmployeeId;
            }

            return baseService.ExecuteNonQuery(criteria);
        }

        public byte GetUserPermissionOnKPI()
        {
            return GetPermissionOnFunction(EPAFunctionKey.JobLevelStandardKPI);
        }
    }
}