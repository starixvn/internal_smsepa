namespace SMS.EPA.Site.Controllers.API
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using HRM.UserControl.Client.Core.Enumerables;
    using SMS.EPA.Core.Attribute;
    using SMS.EPA.Core.Constants;
    using SMS.EPA.Core.Model;
    using SMS.EPA.Core.Services;
    using SMS.EPA.Criteria;
    using SMS.EPA.Model;
    using SMS.EPA.Model.API;
    using SMS.EPA.Services;

    public class DepartmentFlowController : BaseController
    {
        private readonly IDepartmentFlowService departmentFlowService;
        private readonly IBaseService baseService;

        public DepartmentFlowController() : this(null, null)
        {
        }

        public DepartmentFlowController(IDepartmentFlowService departmentFlowService, IBaseService baseService)
        {
            this.departmentFlowService = departmentFlowService ?? new DepartmentFlowService();
            this.baseService = baseService ?? new BaseService();
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<DepartmentFlowModel> GetByFlowTemplateId(int flowTemplateId)
        {
            return departmentFlowService.GetByFlowTemplateId(flowTemplateId);
        }

        [HttpGet]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Read)]
        public IEnumerable<FlowModel> GetById(int departmentId, int flowTemplateId)
        {
            var criteria = new GetDepartmentFlowByIdCriteria
            {
                FlowTemplateId = flowTemplateId,
                DepartmentId = Math.Abs(departmentId),
                IsGroup = departmentId <= 0
            };

            return departmentFlowService.GetById(criteria);
        }

        [HttpPost]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult CreateDepartmentFlow(CreateDepartmentFlowCriteria criteria)
        {
            var commandResult = new CommandResult();
            if (criteria != null)
            {
                criteria.DepartmentId = Math.Abs(criteria.DepartmentId);
                criteria.ModifiedBy = CurrentEmployeeId;
                commandResult = baseService.ExecuteNonQuery(criteria);
            }

            return commandResult;
        }

        [HttpDelete]
        [ApiAuthorize(FunctionKey = EPAFunctionKey.FlowManagement, Policy = Policy.Host)]
        public CommandResult DeleteDepartmentFlow(int departmentId, int flowTemplateId)
        {
            var criteria = new DeleteDepartmentFlowCriteria
            {
                FlowTemplateId = flowTemplateId,
                DepartmentId = Math.Abs(departmentId),
                IsGroup = departmentId <= 0,
                ModifiedBy = CurrentEmployeeId
            };

            return baseService.ExecuteNonQuery(criteria);
        }

        [HttpGet]
        public IEnumerable<Department> GetDepartmentHasSetting(int flowTemplateId)
        {
            var criteria = new GetDepartmentSectionHasSettingCriteria
            {
                FlowTemplateId = flowTemplateId
            };
            return baseService.GetData<Department>(criteria);
        }
    }
}